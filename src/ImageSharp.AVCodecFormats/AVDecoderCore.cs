using System;
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
    private readonly DecoderOptions options;

    private readonly bool preserveAspectRatio;

    public AVDecoderCore(AVDecoderOptions decoderOptions)
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

        options = decoderOptions.GeneralOptions;
        preserveAspectRatio = decoderOptions.PreserveAspectRatio;
    }

    public ImageInfo Identify(Stream stream, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            StreamsToLoad = MediaMode.Video
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

        return new ImageInfo(
            new PixelTypeInfo(bitsPerPixel),
            new Size(file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height),
            new ImageMetadata());
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

    public Image<TPixel> Decode<TPixel>(Stream stream, CancellationToken cancellationToken)
       where TPixel : unmanaged, IPixel<TPixel>
    {
        DrawingSize? targetFrameSize = null;
        if (options.TargetSize != null)
        {
            // Calculate target size with aspect ratio
            if (preserveAspectRatio)
            {
                ImageInfo sourceInfo = Identify(stream, CancellationToken.None);

                var sizeWithAspectRatio = ResizeHelper.CalculateMaxRectangle(
                    sourceInfo.Size,
                    options.TargetSize.Value.Width,
                    options.TargetSize.Value.Height);

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
                    options.TargetSize.Value.Width, 
                    options.TargetSize.Value.Width);
            }
        }

        using var file = MediaFile.Open(stream, new MediaOptions
        {
            // Map imagesharp pixel format to ffmpeg pixel format
            VideoPixelFormat = MapPixelFormat(default(TPixel)),
            TargetVideoSize = targetFrameSize,
            DemuxerOptions = new ContainerOptions
            {
                FlagDiscardCorrupt = true,
            },
            StreamsToLoad = MediaMode.Video,
        });


        Image<TPixel>? resultImage = null;

        uint frameCount = 0;
        try
        {
            while (file.Video.TryGetNextFrame(out var frame))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Image
                if (resultImage == default)
                {
                    resultImage = Image.LoadPixelData<TPixel>(frame.Data, frame.ImageSize.Width, frame.ImageSize.Height);
                }
                // Frame
                else
                {
                    ImageFrame<TPixel> imageFrame = Image.LoadPixelData<TPixel>(
                        frame.Data, 
                        frame.ImageSize.Width, 
                        frame.ImageSize.Height).Frames.RootFrame;

                    resultImage.Frames.AddFrame(imageFrame);
                }

                if (++frameCount == options.MaxFrames)
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

        return resultImage;
    }
}
