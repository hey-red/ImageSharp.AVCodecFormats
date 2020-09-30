using System;

using FFmpeg.AutoGen;

using HeyRed.ImageSharp.AVCodecFormats.Common;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class FrameResampler : IDisposable
    {
        private const int LINESIZE_ALIGNMENT = 32;

        private readonly int _frameWidth;

        private readonly int _frameHeight;

        private readonly AVPixelFormat _destinationPixelFormat;

        private readonly SwsContext* _scaleContext;

        private AVFrame* _destFrame;

        public int BufferSize { get; private set; }

        public FrameResampler(
            int frameWidth,
            int frameHeight,
            AVPixelFormat sourcePixelFormat,
            AVPixelFormat destinationPixelFormat)
        {
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _destinationPixelFormat = destinationPixelFormat;

            _scaleContext = ffmpeg.sws_getCachedContext(
                _scaleContext,
                _frameWidth,
                _frameHeight,
                sourcePixelFormat,
                _frameWidth,
                _frameHeight,
                _destinationPixelFormat,
                ffmpeg.SWS_BICUBIC, null, null, null);
            if (_scaleContext == null)
            {
                throw new AVException("Could not initialize the conversion context.");
            }
        }

        public AVFrame* Resample(AVFrame* sourceFrame)
        {
            _destFrame = ffmpeg.av_frame_alloc();

            var dstData = new byte_ptrArray4();
            var dstStride = new int_array4();

            BufferSize = ffmpeg.av_image_alloc(
                ref dstData,
                ref dstStride,
                _frameWidth,
                _frameHeight,
                _destinationPixelFormat,
                LINESIZE_ALIGNMENT)
                .ThrowExceptionIfError();

            ffmpeg.sws_scale(
                _scaleContext,
                sourceFrame->data,
                sourceFrame->linesize,
                0,
                sourceFrame->height,
                dstData,
                dstStride)
                .ThrowExceptionIfError();

            _destFrame->data.UpdateFrom(dstData);
            _destFrame->linesize.UpdateFrom(dstStride);
            _destFrame->width = _frameWidth;
            _destFrame->height = _frameHeight;
            _destFrame->format = (int)_destinationPixelFormat;

            return _destFrame;
        }

        public void Dispose()
        {
            fixed (AVFrame** dframe = &_destFrame)
            {
                ffmpeg.av_frame_free(dframe);
            }

            ffmpeg.sws_freeContext(_scaleContext);
        }
    }
}