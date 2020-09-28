using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FFmpeg.AutoGen;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public unsafe abstract class BaseAVDecoder : IImageDecoder, IImageInfoDetector
    {
        private static readonly object syncRoot = new object();

        private static bool _initBinaries = false;

        protected BaseAVDecoder()
        {
            if (_initBinaries) return;

            lock (syncRoot)
            {
                if (!_initBinaries)
                {
                    AVBinariesFinder.FindBinaries();

                    _initBinaries = true;
                }
            }
        }

        public virtual Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            using var streamDecoder = new StreamDecoder(stream);
            AVFrame* frame = streamDecoder.DecodeFrame();

            using var frameResampler = new FrameResampler(frame->width, frame->height, (AVPixelFormat)frame->format,
                // FIXME: Some formats can be mapped incorrectly
                destinationPixelFormat: default(TPixel) switch
                {
                    Rgb24 _ => AVPixelFormat.AV_PIX_FMT_RGB24,
                    Argb32 _ => AVPixelFormat.AV_PIX_FMT_ARGB,
                    Rgba32 _ => AVPixelFormat.AV_PIX_FMT_RGBA,
                    Bgra32 _ => AVPixelFormat.AV_PIX_FMT_BGRA,
                    Bgra4444 _ => AVPixelFormat.AV_PIX_FMT_RGB4_BYTE,
                    // BE or LE?
                    Rgba64 _ => AVPixelFormat.AV_PIX_FMT_RGBA64BE,
                    // BE or LE?
                    Bgr565 _ => AVPixelFormat.AV_PIX_FMT_BGR565BE,
                    _ => throw new ArgumentException("Invalid pixel format."),
                });

            AVFrame* resampledFrame = frameResampler.Resample(frame);

            var bytes = new Span<byte>(resampledFrame->data[0], frameResampler.BufferSize);

            return Image.LoadPixelData<TPixel>(bytes, resampledFrame->width, resampledFrame->height);
        }

        public virtual Task<Image<TPixel>> DecodeAsync<TPixel>(Configuration configuration, Stream stream, CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(Decode<TPixel>(configuration, stream));
        }

        public virtual Image Decode(Configuration configuration, Stream stream)
            => Decode<Rgba32>(configuration, stream);

        public virtual Task<Image> DecodeAsync(Configuration configuration, Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(Decode(configuration, stream));
        }

        // TODO: result object instead exception
        public virtual IImageInfo? Identify(Configuration configuration, Stream stream)
        {
            try
            {
                using var streamDecoder = new StreamDecoder(stream);

                return new ImageInfo(
                    streamDecoder.SourceWidth,
                    streamDecoder.SourceHeight);
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return null;
        }

        public virtual Task<IImageInfo?> IdentifyAsync(Configuration configuration, Stream stream, CancellationToken cancellationToken)
            => Task.FromResult(Identify(configuration, stream));
    }
}