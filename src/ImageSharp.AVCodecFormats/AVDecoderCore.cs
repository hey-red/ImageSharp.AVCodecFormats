using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using FFmpeg.AutoGen;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using DrawingSize = System.Drawing.Size;

namespace HeyRed.ImageSharp.AVCodecFormats;

internal sealed unsafe class AVDecoderCore
{
    private static readonly object _syncRoot = new();

    private static bool _initBinaries;

    private readonly DecoderOptions _decoderOptions;

    private readonly AVDecoderOptions _options;

    public AVDecoderCore(AVDecoderOptions avDecoderOptions)
    {
        if (!_initBinaries)
        {
            lock (_syncRoot)
            {
                if (!_initBinaries)
                {
                    FFmpegBinariesFinder.FindBinaries();

                    _initBinaries = true;
                }
            }
        }

        _options = avDecoderOptions;
        _decoderOptions = avDecoderOptions.GeneralOptions;
    }

    public ImageInfo Identify(Stream stream, IImageFormat<AVMetadata> imageFormat, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using MediaFile? file = MediaFile.Open(stream, new MediaOptions
        {
            StreamsToLoad = MediaMode.AudioVideo
        });

        if (!file.HasVideo)
        {
            throw new InvalidDataException("The file has no video streams.");
        }

        var bitsPerPixel = 0;

        AVPixelFormat pixFormat = ffmpeg.av_get_pix_fmt(file.Video.Info.PixelFormat);
        var desc = ffmpeg.av_pix_fmt_desc_get(pixFormat);
        if (desc != null)
        {
            bitsPerPixel = ffmpeg.av_get_bits_per_pixel(desc);
        }

        var metadata = new ImageMetadata();

        FillMetadata(metadata, file, imageFormat);

        return new ImageInfo(
            new PixelTypeInfo(bitsPerPixel),
            new Size(file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height),
            metadata);
    }

    public Image<TPixel> Decode<TPixel>(
        Stream stream,
        IImageFormat<AVMetadata> imageFormat,
        CancellationToken cancellationToken)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var targetFrameSize = CalculateTargetFrameSize(stream, imageFormat);

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            VideoPixelFormat = MapPixelFormat(default(TPixel)),
            TargetVideoSize = targetFrameSize,
            RespectSampleAspectRatio = _options.RespectSampleAspectRatio,
            DemuxerOptions = new ContainerOptions
            {
                FlagDiscardCorrupt = true
            },
            StreamsToLoad = MediaMode.AudioVideo
        });

        Image<TPixel>? resultImage = null;
        uint frameCount = 0;

        int width = file.Video.OutputFrameSize.Width;
        int height = file.Video.OutputFrameSize.Height;
        int bytesPerPixel = Unsafe.SizeOf<TPixel>();

        // reusable scratch buffer for current frame only
        var scratch = ArrayPool<TPixel>.Shared.Rent(width * height);
        try
        {
            // We pin only the current scratch buffer, never the stored result image
            GCHandle pinned = GCHandle.Alloc(scratch, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = pinned.AddrOfPinnedObject();

                if (_options.FramesTimestamps.Length > 0)
                {
                    foreach (TimeSpan ts in _options.FramesTimestamps)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!file.Video.TryGetFrame(ts, ptr, width * bytesPerPixel))
                        {
                            continue;
                        }

                        var current = Image.LoadPixelData<TPixel>(
                            _decoderOptions.Configuration,
                            CopyFromPinnedBuffer(scratch, width, height),
                            width,
                            height);

                        bool mustBeSkipped = _options.FrameFilter?.Invoke(current.Frames.RootFrame, frameCount) is true;
                        if (mustBeSkipped)
                        {
                            current.Dispose();
                            continue;
                        }

                        if (resultImage is null)
                        {
                            resultImage = current;
                        }
                        else
                        {
                            resultImage.Frames.AddFrame(current.Frames.RootFrame);
                        }

                        if (++frameCount >= _decoderOptions.MaxFrames)
                        {
                            break;
                        }
                    }

                    // Fallback: first frame if no timestamp match
                    if (resultImage is null)
                    {
                        file.Reset();

                        if (file.Video.TryGetNextFrame(ptr, width * bytesPerPixel))
                        {
                            resultImage = Image.LoadPixelData(
                                _decoderOptions.Configuration,
                                CopyFromPinnedBuffer(scratch, width, height),
                                width,
                                height);
                            ;
                        }
                    }
                }
                else
                {
                    while (file.Video.TryGetNextFrame(ptr, width * bytesPerPixel))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var current = Image.LoadPixelData<TPixel>(
                            _decoderOptions.Configuration,
                            CopyFromPinnedBuffer(scratch, width, height),
                            width,
                            height);

                        bool mustBeSkipped = _options.FrameFilter?.Invoke(current.Frames.RootFrame, frameCount) is true;
                        if (resultImage is null)
                        {
                            if (!mustBeSkipped || frameCount + 1 == _decoderOptions.MaxFrames)
                            {
                                resultImage = current;
                            }
                            else
                            {
                                current.Dispose();
                            }
                        }
                        else if (!mustBeSkipped)
                        {
                            resultImage.Frames.AddFrame(current.Frames.RootFrame);
                        }

                        if (++frameCount >= _decoderOptions.MaxFrames)
                        {
                            break;
                        }
                    }
                }
            }
            finally
            {
                pinned.Free();
            }
        }
        finally
        {
            ArrayPool<TPixel>.Shared.Return(scratch);
        }

        if (resultImage is null)
        {
            throw new InvalidDataException("No frames found.");
        }

        if (!_decoderOptions.SkipMetadata)
        {
            FillMetadata(resultImage.Metadata, file, imageFormat);
        }

        return resultImage;
    }

    private static TPixel[] CopyFromPinnedBuffer<TPixel>(TPixel[] scratch, int width, int height)
        where TPixel : unmanaged
    {
        var copy = new TPixel[width * height];
        Array.Copy(scratch, copy, copy.Length);
        return copy;
    }

    private static ImagePixelFormat MapPixelFormat<TPixel>(TPixel sourcePixelFormat) => sourcePixelFormat switch
    {
        Rgb24 _ => ImagePixelFormat.Rgb24,
        Bgr24 _ => ImagePixelFormat.Bgr24,
        Rgba32 _ => ImagePixelFormat.Rgba32,
        Argb32 _ => ImagePixelFormat.Argb32,
        Bgra32 _ => ImagePixelFormat.Bgra32,
        _ => throw new ArgumentException("Unsupported pixel format.")
    };

    private static void FillMetadata(ImageMetadata metadata, MediaFile file, IImageFormat<AVMetadata> imageFormat)
    {
        AVMetadata avMetadata = metadata.GetFormatMetadata(imageFormat);

        avMetadata.ContainerFormat = file.Info.ContainerFormat;
        avMetadata.Bitrate = file.Info.Bitrate;
        avMetadata.Duration = file.Info.Duration;
        avMetadata.ContainerMetadata = file.Info.Metadata.Metadata;

        var videoStreams = new List<VideoStreamInfo>();
        var audioStreams = new List<AudioStreamInfo>();

        foreach (VideoStream? videoStream in file.VideoStreams)
        {
            var videStreamInfo = new VideoStreamInfo
            {
                CodecName = videoStream.Info.CodecName,
                Duration = videoStream.Info.Duration,
                AvgFrameRate = videoStream.Info.AvgFrameRate,
                FramesCount = videoStream.Info.NumberOfFrames,
                Rotation = videoStream.Info.Rotation,
                SampleAspectRatio = videoStream.Info.SampleAspectRatio
            };

            videoStreams.Add(videStreamInfo);
        }

        foreach (AudioStream? audioStream in file.AudioStreams)
        {
            var audioStreamInfo = new AudioStreamInfo
            {
                CodecName = audioStream.Info.CodecName,
                Duration = audioStream.Info.Duration,
                NumChannels = audioStream.Info.NumChannels,
                SampleRate = audioStream.Info.SampleRate
            };

            audioStreams.Add(audioStreamInfo);
        }

        avMetadata.VideoStreams = videoStreams;
        avMetadata.AudioStreams = audioStreams;
    }

    private DrawingSize? CalculateTargetFrameSize(Stream stream, IImageFormat<AVMetadata> imageFormat)
    {
        DrawingSize? targetFrameSize = null;
        if (_decoderOptions.TargetSize == null)
        {
            return targetFrameSize;
        }

        // Calculate target size with aspect ratio
        if (_options.PreserveAspectRatio)
        {
            ImageInfo sourceInfo = Identify(stream, imageFormat, CancellationToken.None);

            Size sizeWithAspectRatio = ResizeHelper.CalculateMaxRectangle(
                sourceInfo.Size,
                _decoderOptions.TargetSize.Value.Width,
                _decoderOptions.TargetSize.Value.Height);

            targetFrameSize = new DrawingSize(
                sizeWithAspectRatio.Width,
                sizeWithAspectRatio.Height);

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
        }
        else
        {
            targetFrameSize = new DrawingSize(
                _decoderOptions.TargetSize.Value.Width,
                _decoderOptions.TargetSize.Value.Height);
        }

        return targetFrameSize;
    }
}