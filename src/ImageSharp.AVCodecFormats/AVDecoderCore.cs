using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using DrawingSize = System.Drawing.Size;

namespace HeyRed.ImageSharp.AVCodecFormats;

internal sealed class AVDecoderCore
{
    private static readonly object _syncRoot = new();
    private static bool _initBinaries;

    private readonly DecoderOptions _decoderOptions;
    private readonly AVDecoderOptions _options;

    public AVDecoderCore(AVDecoderOptions avDecoderOptions)
    {
        if (!_initBinaries)
            lock (_syncRoot)
            {
                if (!_initBinaries)
                {
                    FFmpegBinariesFinder.FindBinaries();
                    _initBinaries = true;
                }
            }

        _options = avDecoderOptions;
        _decoderOptions = avDecoderOptions.GeneralOptions;
    }

    public ImageInfo Identify(Stream stream, IImageFormat<AVMetadata> imageFormat, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            StreamsToLoad = MediaMode.AudioVideo
        });

        if (!file.HasVideo) throw new InvalidDataException("The file has no video streams.");

        var metadata = new ImageMetadata();
        FillMetadata(metadata, file, imageFormat);

        return new ImageInfo(
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
            StreamsToLoad = MediaMode.AudioVideo,
            DecoderThreads = _options.DecoderThreads
        });

        var width = file.Video.OutputFrameSize.Width;
        var height = file.Video.OutputFrameSize.Height;
        var bytesPerPixel = Unsafe.SizeOf<TPixel>();
        var totalPixels = width * height;

        // Reusable frame buffer.
        var scratch = ArrayPool<TPixel>.Shared.Rent(totalPixels);
        ReadOnlySpan<TPixel> scratchSpan = scratch.AsSpan(0, totalPixels);

        Image<TPixel>? resultImage = null;
        uint frameIndex = 0;
        var frameCounter = 0;

        try
        {
            var pin = GCHandle.Alloc(scratch, GCHandleType.Pinned);
            try
            {
                var ptr = pin.AddrOfPinnedObject();

                if (_options.FramesTimestamps.Length > 0)
                {
                    foreach (var ts in _options.FramesTimestamps)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!file.Video.TryGetFrame(ts, ptr, width * bytesPerPixel)) continue;

                        var current = LoadFrame(
                            _decoderOptions.Configuration,
                            scratchSpan,
                            width,
                            height,
                            frameIndex);

                        var mustBeSkipped = _options.FrameFilter?.Invoke(current.Frames.RootFrame, frameIndex) is true;
                        if (mustBeSkipped)
                        {
                            current.Dispose();
                            continue;
                        }

                        if (resultImage is null)
                            resultImage = current;
                        else
                            resultImage.Frames.AddFrame(current.Frames.RootFrame);

                        frameIndex++;
                        frameCounter++;

                        if (frameCounter >= _decoderOptions.MaxFrames) break;
                    }

                    // Fallback if no timestamp matched.
                    if (resultImage is null)
                    {
                        file.Reset();

                        if (file.Video.TryGetNextFrame(ptr, width * bytesPerPixel))
                        {
                            var current = LoadFrame(
                                _decoderOptions.Configuration,
                                scratchSpan,
                                width,
                                height,
                                frameIndex);

                            resultImage = current;
                        }
                    }
                }
                else
                {
                    while (file.Video.TryGetNextFrame(ptr, width * bytesPerPixel))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var current = LoadFrame(
                            _decoderOptions.Configuration,
                            scratchSpan,
                            width,
                            height,
                            frameIndex);

                        var mustBeSkipped = _options.FrameFilter?.Invoke(current.Frames.RootFrame, frameIndex) is true;

                        if (mustBeSkipped)
                            current.Dispose();
                        else if (resultImage is null)
                            resultImage = current;
                        else
                            resultImage.Frames.AddFrame(current.Frames.RootFrame);

                        frameIndex++;
                        frameCounter++;

                        if (frameCounter >= _decoderOptions.MaxFrames) break;
                    }
                }
            }
            finally
            {
                pin.Free();
            }
        }
        finally
        {
            ArrayPool<TPixel>.Shared.Return(scratch);
        }

        if (resultImage is null) throw new InvalidDataException("No frames found.");

        if (!_decoderOptions.SkipMetadata) FillMetadata(resultImage.Metadata, file, imageFormat);

        return resultImage;
    }

    private static Image<TPixel> LoadFrame<TPixel>(
        Configuration configuration,
        ReadOnlySpan<TPixel> scratchSpan,
        int width,
        int height,
        uint frameIndex)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        // This avoids creating a second temp array.
        // The frame buffer is already in the rented array and is valid for the duration of this call.
        var image = Image.LoadPixelData(
            configuration,
            scratchSpan,
            width,
            height);

        // We don't mutate frameIndex here, but keeping it as a parameter makes this helper
        // easier to extend to logging/debugging later without broad changes.
        _ = frameIndex;
        return image;
    }

    private static ImagePixelFormat MapPixelFormat<TPixel>(TPixel sourcePixelFormat)
        where TPixel : unmanaged
    {
        return sourcePixelFormat switch
        {
            Rgb24 _ => ImagePixelFormat.Rgb24,
            Bgr24 _ => ImagePixelFormat.Bgr24,
            Rgba32 _ => ImagePixelFormat.Rgba32,
            Argb32 _ => ImagePixelFormat.Argb32,
            Bgra32 _ => ImagePixelFormat.Bgra32,
            _ => throw new ArgumentException("Unsupported pixel format.")
        };
    }

    private static void FillMetadata(ImageMetadata metadata, MediaFile file, IImageFormat<AVMetadata> imageFormat)
    {
        var avMetadata = metadata.GetFormatMetadata(imageFormat);

        avMetadata.ContainerFormat = file.Info.ContainerFormat;
        avMetadata.Bitrate = file.Info.Bitrate;
        avMetadata.Duration = file.Info.Duration;
        avMetadata.ContainerMetadata = file.Info.Metadata.Metadata;
        avMetadata.PixelFormat = file.Video.Info.PixelFormat;

        var videoStreams = new List<VideoStreamInfo>();
        var audioStreams = new List<AudioStreamInfo>();

        foreach (var videoStream in file.VideoStreams)
        {
            var videoStreamInfo = new VideoStreamInfo
            {
                CodecName = videoStream.Info.CodecName,
                Duration = videoStream.Info.Duration,
                AvgFrameRate = videoStream.Info.AvgFrameRate,
                FramesCount = videoStream.Info.NumberOfFrames,
                Rotation = videoStream.Info.Rotation,
                SampleAspectRatio = videoStream.Info.SampleAspectRatio
            };

            videoStreams.Add(videoStreamInfo);
        }

        foreach (var audioStream in file.AudioStreams)
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
        if (_decoderOptions.TargetSize == null) return null;

        if (_options.PreserveAspectRatio)
        {
            var sourceInfo = Identify(stream, imageFormat, CancellationToken.None);

            var sizeWithAspectRatio = ResizeHelper.CalculateMaxRectangle(
                sourceInfo.Size,
                _decoderOptions.TargetSize.Value.Width,
                _decoderOptions.TargetSize.Value.Height);

            if (stream.CanSeek) stream.Position = 0;

            return new DrawingSize(sizeWithAspectRatio.Width, sizeWithAspectRatio.Height);
        }

        return new DrawingSize(
            _decoderOptions.TargetSize.Value.Width,
            _decoderOptions.TargetSize.Value.Height);
    }
}