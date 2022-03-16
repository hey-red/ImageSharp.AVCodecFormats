using System;
using System.IO;
using System.Threading;

using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;

using HeyRed.ImageSharp.AVCodecFormats.Common;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

using DrawingSize = System.Drawing.Size;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public abstract unsafe class BaseAVDecoder : IImageDecoder, IImageInfoDetector
    {
        private static readonly object syncRoot = new();

        private static bool _initBinaries = false;

        private readonly BlackFrameFilter? _blackFrameFilter;

        protected BaseAVDecoder(IAVDecoderOptions decoderOptions) : this()
        {
            _options = decoderOptions ?? throw new ArgumentNullException(nameof(decoderOptions));

            if (_options.BlackFilterOptions != null)
            {
                _blackFrameFilter = new BlackFrameFilter(_options.BlackFilterOptions);
            }
        }

        protected BaseAVDecoder()
        {
            if (_initBinaries) return;

            lock (syncRoot)
            {
                if (!_initBinaries)
                {
                    FFmpegBinariesFinder.FindBinaries();

                    _initBinaries = true;
                }
            }
        }

        private readonly IAVDecoderOptions? _options;

        private DrawingSize CalculateSizeWithAspectRatio(Size sourceSize, int size)
        {
            double ratio = 1;
            if (sourceSize.Width > size || sourceSize.Height > size)
            {
                var ratioX = size / (double)sourceSize.Width;
                var ratioY = size / (double)sourceSize.Height;
                ratio = Math.Min(ratioX, ratioY);
            }

            return new(
                (int)Math.Round(sourceSize.Width * ratio),
                (int)Math.Round(sourceSize.Height * ratio));
        }

        private DrawingSize? CalculateTargetSize(Configuration configuration, Stream stream)
        {
            DrawingSize? targetFrameSize = null;
            if (_options?.FrameSizeOptions != null)
            {
                (int targetWidth, int targetHeight) = _options.FrameSizeOptions.TargetFrameSize;
                // Calculate frames size with aspect ratio
                if (_options.FrameSizeOptions.PreserveAspectRatio)
                {
                    IImageInfo? sourceInfo = Identify(configuration, stream, CancellationToken.None);
                    if (sourceInfo is not null)
                    {
                        int size = Math.Max(targetWidth, targetHeight);
                        targetFrameSize = CalculateSizeWithAspectRatio(new Size(sourceInfo.Width, sourceInfo.Height), size);
                    }

                    if (stream.CanSeek)
                    {
                        stream.Position = 0;
                    }
                }
                else
                {
                    targetFrameSize = new DrawingSize(targetWidth, targetHeight);
                }
            }
            return targetFrameSize;
        }

        private ImagePixelFormat MapPixelFormat<TPixel>(TPixel sourcePixelFormat) => sourcePixelFormat switch
        {
            Rgb24 _ => ImagePixelFormat.Rgb24,
            Bgr24 _ => ImagePixelFormat.Bgr24,
            Rgba32 _ => ImagePixelFormat.Rgba32,
            Argb32 _ => ImagePixelFormat.Argb32,
            Bgra32 _ => ImagePixelFormat.Bgra32,
            _ => throw new ArgumentException("Unsupported pixel format."),
        };

        public virtual Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
        {
            DrawingSize? targetFrameSize = CalculateTargetSize(configuration, stream);

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

            ImageData lastDecodedFrame = default;

            int decodedFrames = 0;
            while (file.Video.TryGetNextFrame(out var frame))
            {
                decodedFrames++;

                lastDecodedFrame = frame;

                // Filter black frames
                if (_blackFrameFilter != null &&
                    decodedFrames < _options?.BlackFilterOptions!.FramesLimit)
                {
                    if (_blackFrameFilter!.IsBlackFrame(frame)) continue;
                }

                break;
            }

            if (lastDecodedFrame.Data.IsEmpty)
            {
                throw new InvalidDataException("No frames found.");
            }

            return Image.LoadPixelData<TPixel>(lastDecodedFrame.Data, lastDecodedFrame.ImageSize.Width, lastDecodedFrame.ImageSize.Height);
        }

        public virtual Image Decode(Configuration configuration, Stream stream, CancellationToken cancellationToken) => Decode<Rgb24>(configuration, stream, cancellationToken);

        public virtual IImageInfo? Identify(Configuration configuration, Stream stream, CancellationToken cancellationToken)
        {
            using var file = MediaFile.Open(stream, new MediaOptions
            {
                StreamsToLoad = MediaMode.Video
            });

            if (file.HasVideo)
            {
                return new ImageInfo(file.Video.Info.FrameSize.Width, file.Video.Info.FrameSize.Height);
            }

            return null;
        }
    }
}