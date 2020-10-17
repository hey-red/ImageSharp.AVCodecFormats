using System;

using FFmpeg.AutoGen;

using HeyRed.ImageSharp.AVCodecFormats.Common;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class FrameResampler : IDisposable
    {
        private readonly SwsContext* _scaleContext;

        private readonly int _destStride;

        public FrameResampler(
            int frameWidth,
            int frameHeight,
            AVPixelFormat sourcePixelFormat,
            AVPixelFormat destinationPixelFormat)
        {
            _destStride = (int)GetBytesPerPixel(destinationPixelFormat) * frameWidth;

            _scaleContext = ffmpeg.sws_getContext(
                frameWidth,
                frameHeight,
                sourcePixelFormat,
                frameWidth,
                frameHeight,
                destinationPixelFormat,
                ffmpeg.SWS_BICUBIC, null, null, null);
            if (_scaleContext == null)
            {
                throw new AVException("Could not initialize the conversion context.");
            }
        }

        public Span<byte> Resample(AVFrame* sourceFrame)
        {
            int bufferSize = _destStride * sourceFrame->height;

            Span<byte> span = new byte[bufferSize];

            fixed (byte* ptr = span)
            {
                var data = new byte*[4] { ptr, null, null, null };

                var linesize = new int[4] { _destStride, 0, 0, 0 };

                ffmpeg.sws_scale(
                    _scaleContext,
                    sourceFrame->data,
                    sourceFrame->linesize,
                    0,
                    sourceFrame->height,
                    data,
                    linesize)
                    .ThrowExceptionIfError();
            }

            return span;
        }

        private double GetBytesPerPixel(AVPixelFormat format) => format switch
        {
            AVPixelFormat.AV_PIX_FMT_RGBA => 4,
            AVPixelFormat.AV_PIX_FMT_ARGB => 4,
            AVPixelFormat.AV_PIX_FMT_BGRA => 4,
            AVPixelFormat.AV_PIX_FMT_RGB24 => 3,
            _ => 0
        };

        public void Dispose() => ffmpeg.sws_freeContext(_scaleContext);
    }
}