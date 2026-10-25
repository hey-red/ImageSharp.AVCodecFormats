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
            StreamsToLoad = MediaMode.AudioVideo,
            // Identify never decodes frames, so there is no reason to start decoder threads. Without this the
            // library default (automatic threading) would create them for nothing on every call.
            DecoderThreads = 1
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
        var targetFrameSize = CalculateTargetFrameSize(stream);

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            VideoPixelFormat = MapPixelFormat(default(TPixel)),
            TargetVideoSize = targetFrameSize,
            RespectSampleAspectRatio = _options.RespectSampleAspectRatio,
            DemuxerOptions = new ContainerOptions
            {
                FlagDiscardCorrupt = true
            },
            // Audio streams are only needed to fill the metadata.
            StreamsToLoad = _decoderOptions.SkipMetadata ? MediaMode.Video : MediaMode.AudioVideo,
            DecoderThreads = _options.DecoderThreads
        });

        if (!file.HasVideo) throw new InvalidDataException("The file has no video streams.");

        var width = file.Video.OutputFrameSize.Width;
        var height = file.Video.OutputFrameSize.Height;
        var bytesPerPixel = Unsafe.SizeOf<TPixel>();
        var stride = width * bytesPerPixel;
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

                        if (!file.Video.TryGetFrame(ts, ptr, stride)) continue;
                        
                        var kept = AppendFrame(
                            ref resultImage,
                            _decoderOptions.Configuration,
                            scratchSpan,
                            width,
                            height,
                            _options.FrameFilter,
                            frameIndex);

                        if (!kept) continue;

                        frameIndex++;
                        frameCounter++;

                        if (frameCounter >= _decoderOptions.MaxFrames) break;
                    }

                    // Fallback if no timestamp matched.
                    if (resultImage is null)
                    {
                        file.Reset();

                        if (file.Video.TryGetNextFrame(ptr, stride))
                        {
                            resultImage = Image.LoadPixelData(
                                _decoderOptions.Configuration,
                                scratchSpan,
                                width,
                                height);
                        }
                    }
                }
                else
                {
                    while (file.Video.TryGetNextFrame(ptr, stride))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        AppendFrame(
                            ref resultImage,
                            _decoderOptions.Configuration,
                            scratchSpan,
                            width,
                            height,
                            _options.FrameFilter,
                            frameIndex);
                        
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

            if (resultImage is null) throw new InvalidDataException("No frames found.");

            if (!_decoderOptions.SkipMetadata) FillMetadata(resultImage.Metadata, file, imageFormat);

            return resultImage;
        }
        catch
        {
            resultImage?.Dispose();
            throw;
        }
        finally
        {
            ArrayPool<TPixel>.Shared.Return(scratch);
        }
    }

    /// <summary>
    ///     Adds the frame that is currently stored in <paramref name="pixels" /> to the result image.
    /// </summary>
    /// <returns><see langword="true" /> if the frame was kept, <see langword="false" /> if the filter rejected it.</returns>
    private static bool AppendFrame<TPixel>(
        ref Image<TPixel>? resultImage,
        Configuration configuration,
        ReadOnlySpan<TPixel> pixels,
        int width,
        int height,
        Func<ImageFrame, uint, bool>? frameFilter,
        uint frameIndex)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        if (resultImage is null)
        {
            // An Image cannot exist without a frame, so the first frame has to be created as a standalone image.
            var first = Image.LoadPixelData(configuration, pixels, width, height);

            if (frameFilter?.Invoke(first.Frames.RootFrame, frameIndex) is true)
            {
                first.Dispose();
                return false;
            }

            resultImage = first;
            return true;
        }

        // Copy the pixels straight into a new frame of the result image.
        var frame = resultImage.Frames.AddFrame(pixels);

        if (frameFilter?.Invoke(frame, frameIndex) is true)
        {
            // The result image already has at least one frame, so removing this one is allowed.
            resultImage.Frames.RemoveFrame(resultImage.Frames.Count - 1);
            return false;
        }

        return true;
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

    private DrawingSize? CalculateTargetFrameSize(Stream stream)
    {
        if (_decoderOptions.TargetSize == null) return null;

        if (_options.PreserveAspectRatio)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("PreserveAspectRatio requires a seekable stream.");

            var startPosition = stream.Position;

            var sourceSize = ReadVideoFrameSize(stream);

            stream.Position = startPosition;

            var sizeWithAspectRatio = ResizeHelper.CalculateMaxRectangle(
                sourceSize,
                _decoderOptions.TargetSize.Value.Width,
                _decoderOptions.TargetSize.Value.Height);

            return new DrawingSize(sizeWithAspectRatio.Width, sizeWithAspectRatio.Height);
        }

        return new DrawingSize(
            _decoderOptions.TargetSize.Value.Width,
            _decoderOptions.TargetSize.Value.Height);
    }

    /// <summary>
    ///     Reads only the frame size of the first video stream.
    /// </summary>
    private static Size ReadVideoFrameSize(Stream stream)
    {
        using var file = MediaFile.Open(stream, new MediaOptions
        {
            StreamsToLoad = MediaMode.Video,
            DecoderThreads = 1
        });

        if (!file.HasVideo) throw new InvalidDataException("The file has no video streams.");

        var size = file.Video.Info.FrameSize;

        return new Size(size.Width, size.Height);
    }
}