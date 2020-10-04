using System;
using System.Runtime.InteropServices;

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

            BufferSize = ffmpeg.av_image_get_buffer_size(destinationPixelFormat, _frameWidth, _frameHeight, LINESIZE_ALIGNMENT);

            _bufferPtr = Marshal.AllocHGlobal(BufferSize);
            _dstData = new byte_ptrArray4();
            _dstLinesize = new int_array4();

            ffmpeg.av_image_fill_arrays(ref _dstData, ref _dstLinesize, (byte*)_bufferPtr, destinationPixelFormat, _frameWidth, _frameHeight, LINESIZE_ALIGNMENT);
        }

        private readonly IntPtr _bufferPtr;

        private readonly byte_ptrArray4 _dstData;

        private readonly int_array4 _dstLinesize;

        public AVFrame Resample(AVFrame* sourceFrame)
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

            var frame = new AVFrame();

            frame.data.UpdateFrom(_dstData);
            frame.linesize.UpdateFrom(_dstLinesize);
            frame.width = _frameWidth;
            frame.height = _frameHeight;
            frame.format = (int)_destinationPixelFormat;

            return frame;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_bufferPtr);
            ffmpeg.sws_freeContext(_scaleContext);
        }
    }
}