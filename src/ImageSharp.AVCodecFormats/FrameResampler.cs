using System;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

using HeyRed.ImageSharp.AVCodecFormats.Common;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class FrameResampler : IDisposable
    {
        private const int LINESIZE_ALIGNMENT = 32;

        private readonly SwsContext* _scaleContext;

        private readonly int _bufferSize;

        private readonly void* _bufferPtr;

        private readonly byte_ptrArray4 _dstData;

        private readonly int_array4 _dstLinesize;

        public FrameResampler(
            int frameWidth,
            int frameHeight,
            AVPixelFormat sourcePixelFormat,
            AVPixelFormat destinationPixelFormat)
        {
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

            _bufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat, frameWidth, frameHeight, LINESIZE_ALIGNMENT)
                .ThrowExceptionIfError();

            _bufferPtr = ffmpeg.av_malloc((ulong)_bufferSize);
            if (_bufferPtr == null)
            {
                throw new AVException("Cannot allocate memory buffer.");
            }

            _dstData = new byte_ptrArray4();
            _dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLinesize, (byte*)_bufferPtr, destinationPixelFormat, frameWidth, frameHeight, LINESIZE_ALIGNMENT)
                .ThrowExceptionIfError();
        }

        public Span<byte> Resample(AVFrame* sourceFrame)
        {
            ffmpeg.sws_scale(
                _scaleContext,
                sourceFrame->data,
                sourceFrame->linesize,
                0,
                sourceFrame->height,
                _dstData,
                _dstLinesize)
                .ThrowExceptionIfError();

            // Just copy to managed array
            var buffer = new byte[_bufferSize];

            Marshal.Copy((IntPtr)_bufferPtr, buffer, 0, _bufferSize);

            return new Span<byte>(buffer);
        }

        public void Dispose()
        {
            ffmpeg.av_free(_bufferPtr);
            ffmpeg.sws_freeContext(_scaleContext);
        }
    }
}