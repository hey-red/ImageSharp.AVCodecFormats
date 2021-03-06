﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;

using HeyRed.ImageSharp.AVCodecFormats.Common;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

using DrawingSize = System.Drawing.Size;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public unsafe abstract class BaseAVDecoder : IImageDecoder, IImageInfoDetector
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
                if (_options.FrameSizeOptions.PreserveAspectRation)
                {
                    IImageInfo? sourceInfo = Identify(configuration, stream);
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

        public virtual Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            lock (syncRoot)
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

                ImageData frame;

                // Filter black frames
                if (_options?.BlackFilterOptions != null &&
                    _blackFrameFilter != null)
                {
                    bool isBlackFrame = false;

                    int decodedFramesCounter = 0;
                    do
                    {
                        if (file.Video.TryGetNextFrame(out frame))
                        {
                            isBlackFrame = _blackFrameFilter.IsBlackFrame(frame);
                        }
                        else
                        {
                            break;
                        }

                        decodedFramesCounter++;
                    }
                    while (isBlackFrame && decodedFramesCounter <= _options.BlackFilterOptions.FramesLimit);
                }
                else
                {
                    file.Video.TryGetNextFrame(out frame);
                }

                return Image.LoadPixelData<TPixel>(frame.Data, frame.ImageSize.Width, frame.ImageSize.Height);
            }
        }

        public virtual Image Decode(Configuration configuration, Stream stream) => Decode<Rgb24>(configuration, stream);

        public virtual Task<Image<TPixel>> DecodeAsync<TPixel>(Configuration configuration, Stream stream, CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(Decode<TPixel>(configuration, stream));
        }

        public virtual Task<Image> DecodeAsync(Configuration configuration, Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(Decode(configuration, stream));
        }

        public virtual IImageInfo? Identify(Configuration configuration, Stream stream)
        {
            lock (syncRoot)
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

        public virtual Task<IImageInfo?> IdentifyAsync(Configuration configuration, Stream stream, CancellationToken cancellationToken)
            => Task.FromResult(Identify(configuration, stream));
    }
}