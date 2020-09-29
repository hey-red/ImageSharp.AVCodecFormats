using System;
using System.IO;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class StreamDecoder : IDisposable
    {
        private readonly Stream _inputStream;

        private readonly AVCodecContext* _codecContext;

        private readonly AVFormatContext* _formatContext;

        private readonly int _streamIndex;

        private readonly AVFrame* _frame;

        private readonly AVPacket* _packet;

        private const int AVIO_CTX_BUFFER_SIZE = 4096;

        private readonly byte* _avioBuffer;

        private int ReadPacket(void* opaque, byte* buf, int bufSize)
        {
            byte[] tempBuf = new byte[bufSize];
            int readed = _inputStream.Read(tempBuf, 0, bufSize);
            Marshal.Copy(tempBuf, 0, (IntPtr)buf, readed);
            return readed;
        }

        private long Seek(void* opaque, long offset, int whence)
        {
            // AVSEEK_SIZE
            if (whence == 0x10000)
            {
                if (_inputStream is MemoryStream)
                {
                    return _inputStream.Length;
                }
                return -1;
            }
            if (!_inputStream.CanSeek)
            {
                return -1;
            }

            return whence switch
            {
                // SEEK_SET
                0 => _inputStream.Seek(offset, SeekOrigin.Begin),
                // SEEK_CUR
                1 => _inputStream.Seek(offset, SeekOrigin.Current),
                // SEEK_END
                2 => _inputStream.Seek(offset, SeekOrigin.End),
                _ => -1,
            };
        }

        public int SourceWidth { get; }

        public int SourceHeight { get; }

        public StreamDecoder(Stream inputStream)
        {
            _inputStream = inputStream;

            _formatContext = ffmpeg.avformat_alloc_context();
            if (_formatContext == null)
            {
                throw new InvalidOperationException("Cannot allocate FormatContext.");
            }

            // Discard frames marked as corrupted
            _formatContext->flags |= ffmpeg.AVFMT_FLAG_DISCARD_CORRUPT;

            _avioBuffer = (byte*)ffmpeg.av_malloc(AVIO_CTX_BUFFER_SIZE);

            _formatContext->pb = ffmpeg.avio_alloc_context(
                _avioBuffer, AVIO_CTX_BUFFER_SIZE, 0, null,
                (avio_alloc_context_read_packet)ReadPacket, null,
                (avio_alloc_context_seek)Seek);
            if (_formatContext->pb == null)
            {
                throw new InvalidOperationException("Cannot allocate AVIOContext.");
            }

            var pFormatContext = _formatContext;
            ffmpeg.avformat_open_input(&pFormatContext, null, null, null)
                .ThrowExceptionIfError();

            ffmpeg.avformat_find_stream_info(_formatContext, null)
                .ThrowExceptionIfError();

            AVCodec* codec = null;
            _streamIndex = ffmpeg.av_find_best_stream(_formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0)
                .ThrowExceptionIfError();

            _codecContext = ffmpeg.avcodec_alloc_context3(codec);
            if (_codecContext == null)
            {
                throw new InvalidOperationException("Cannot allocate codec context.");
            }

            // Copy stream parameters
            ffmpeg.avcodec_parameters_to_context(_codecContext, _formatContext->streams[_streamIndex]->codecpar);

            ffmpeg.avcodec_open2(_codecContext, codec, null)
                .ThrowExceptionIfError();

            SourceWidth = _codecContext->width;
            SourceHeight = _codecContext->height;

            _packet = ffmpeg.av_packet_alloc();
            if (_packet == null)
            {
                throw new AVException("Cannot allocate packet.");
            }

            _frame = ffmpeg.av_frame_alloc();
            if (_packet == null)
            {
                throw new AVException("Cannot allocate frame.");
            }
        }

        // TODO: https://github.com/Ruslan-B/FFmpeg.AutoGen/issues/112#issuecomment-491901341
        public AVFrame* DecodeFrame()
        {
            int error;
            do
            {
                do
                {
                    ffmpeg.av_packet_unref(_packet);

                    error = ffmpeg.av_read_frame(_formatContext, _packet);
                    if (error == ffmpeg.AVERROR_EOF)
                    {
                        return _frame;
                    }

                    error.ThrowExceptionIfError();
                }
                while (_packet->stream_index != _streamIndex);

                ffmpeg.avcodec_send_packet(_codecContext, _packet).ThrowExceptionIfError();

                error = ffmpeg.avcodec_receive_frame(_codecContext, _frame);
            }
            while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            error.ThrowExceptionIfError();

            return _frame;
        }

        public void Dispose()
        {
            //TODO: ffmpeg.av_freep(_avioBuffer);

            ffmpeg.av_frame_unref(_frame);
            ffmpeg.av_freep(_frame);

            ffmpeg.av_packet_unref(_packet);
            ffmpeg.av_freep(_packet);

            fixed (AVCodecContext** codecContext = &_codecContext)
            {
                ffmpeg.avcodec_free_context(codecContext);
            }

            // avformat_free_context is not required here
            var pFormatContext = _formatContext;
            ffmpeg.avformat_close_input(&pFormatContext);
        }
    }
}