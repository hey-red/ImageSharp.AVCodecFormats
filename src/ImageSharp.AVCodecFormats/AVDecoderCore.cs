using System;
using System.Collections.Generic;
using System.IO;
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

internal unsafe sealed class AVDecoderCore
{
    private static readonly object syncRoot = new();

    private static bool initBinaries = false;

    /// <inheritdoc />
    private readonly DecoderOptions decoderOptions;

    private readonly AVDecoderOptions options;

    public AVDecoderCore(AVDecoderOptions avDecoderOptions)
    {
        if (!initBinaries)
        {
            lock (syncRoot)
            {
                if (!initBinaries)
                {
                    FFmpegBinariesFinder.FindBinaries();

                    initBinaries = true;
                }
            }
        }

        options = avDecoderOptions;
        decoderOptions = avDecoderOptions.GeneralOptions;
    }

    public ImageInfo Identify(Stream stream, IImageFormat<AVMetadata> imageFormat, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            StreamsToLoad = MediaMode.AudioVideo
        });

        if (!file.HasVideo)
        {
            throw new InvalidDataException("The file has no video streams.");
        }

        int bitsPerPixel = 0;

        AVPixelFormat pixFormat = ffmpeg.av_get_pix_fmt(file.Video.Info.PixelFormat);
        AVPixFmtDescriptor* desc = ffmpeg.av_pix_fmt_desc_get(pixFormat);
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

    public Image<TPixel> Decode<TPixel>(Stream stream, IImageFormat<AVMetadata> imageFormat, CancellationToken cancellationToken)
       where TPixel : unmanaged, IPixel<TPixel>
    {
        DrawingSize? targetFrameSize = CalculateTargetFrameSize(stream, imageFormat);

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            // Map imagesharp pixel format to ffmpeg pixel format
            VideoPixelFormat = MapPixelFormat(default(TPixel)),
            TargetVideoSize = targetFrameSize,
            DemuxerOptions = new ContainerOptions
            {
                FlagDiscardCorrupt = true,
            },
            StreamsToLoad = MediaMode.AudioVideo,
        });

        Image<TPixel>? resultImage = null;

        uint frameCount = 0;
        try
        {
            while (file.Video.TryGetNextFrame(out ImageData frame))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (resultImage == default)
                {
                    resultImage = Image.LoadPixelData<TPixel>(
                        decoderOptions.Configuration,
                        frame.Data,
                        frame.ImageSize.Width,
                        frame.ImageSize.Height);

                    if (options.FrameFilter?.Invoke(resultImage.Frames.RootFrame, frameCount) is true)
                    {
                        if (frameCount + 1 != decoderOptions.MaxFrames)
                        {
                            resultImage.Dispose();
                            resultImage = null;
                        }
                    }
                }
                else
                {
                    using var image = Image.LoadPixelData<TPixel>(
                        decoderOptions.Configuration,
                        frame.Data,
                        frame.ImageSize.Width,
                        frame.ImageSize.Height);

                    if (options.FrameFilter is null ||
                        options.FrameFilter?.Invoke(image.Frames.RootFrame, frameCount) is false)
                    {
                        resultImage.Frames.AddFrame(image.Frames.RootFrame);
                    }
                }

                if (++frameCount == decoderOptions.MaxFrames)
                {
                    break;
                }
            }

            if (resultImage == null)
            {
                throw new InvalidDataException("No frames found.");
            }
        }
        catch
        {
            resultImage?.Dispose();

            throw;
        }

        if (!decoderOptions.SkipMetadata)
        {
            FillMetadata(resultImage.Metadata, file, imageFormat);
        }

        return resultImage;
    }

    private static ImagePixelFormat MapPixelFormat<TPixel>(TPixel sourcePixelFormat) => sourcePixelFormat switch
    {
        Rgb24 _ => ImagePixelFormat.Rgb24,
        Bgr24 _ => ImagePixelFormat.Bgr24,
        Rgba32 _ => ImagePixelFormat.Rgba32,
        Argb32 _ => ImagePixelFormat.Argb32,
        Bgra32 _ => ImagePixelFormat.Bgra32,
        _ => throw new ArgumentException("Unsupported pixel format."),
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

        foreach (var videoStream in file.VideoStreams)
        {
            var videStreamInfo = new VideoStreamInfo()
            {
                CodecName = videoStream.Info.CodecName,
                Duration = videoStream.Info.Duration,
                AvgFrameRate = videoStream.Info.AvgFrameRate,
                FramesCount = videoStream.Info.NumberOfFrames,
                Rotation = videoStream.Info.Rotation,
            };

            videoStreams.Add(videStreamInfo);
        }

        foreach (var audioStream in file.AudioStreams)
        {
            var audioStreamInfo = new AudioStreamInfo()
            {
                CodecName = audioStream.Info.CodecName,
                Duration = audioStream.Info.Duration,
                NumChannels = audioStream.Info.NumChannels,
                SampleRate = audioStream.Info.SampleRate,
            };

            audioStreams.Add(audioStreamInfo);
        }

        avMetadata.VideoStreams = videoStreams;
        avMetadata.AudioStreams = audioStreams;
    }

    private DrawingSize? CalculateTargetFrameSize(Stream stream, IImageFormat<AVMetadata> imageFormat)
    {
        DrawingSize? targetFrameSize = null;
        if (decoderOptions.TargetSize != null)
        {
            // Calculate target size with aspect ratio
            if (options.PreserveAspectRatio)
            {
                ImageInfo sourceInfo = Identify(stream, imageFormat, CancellationToken.None);

                var sizeWithAspectRatio = ResizeHelper.CalculateMaxRectangle(
                    sourceInfo.Size,
                    decoderOptions.TargetSize.Value.Width,
                    decoderOptions.TargetSize.Value.Height);

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
                    decoderOptions.TargetSize.Value.Width,
                    decoderOptions.TargetSize.Value.Width);
            }
        }

        return targetFrameSize;
    }
}
