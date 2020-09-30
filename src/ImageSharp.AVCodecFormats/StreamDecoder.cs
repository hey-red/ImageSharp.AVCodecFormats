using System;

using FFmpeg.AutoGen;

using HeyRed.ImageSharp.AVCodecFormats.Common;
using HeyRed.ImageSharp.AVCodecFormats.IO;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class StreamDecoder : IDisposable
    {
        private FrameFilter? _frameFilter;

        private readonly IAvioStream _inputStream;

        private readonly AVCodecContext* _codecContext;

        private readonly AVFormatContext* _formatContext;

        private readonly int _streamIndex;

        private AVFrame* _frame;

        private AVPacket* _packet;

        public int SourceWidth { get; }

        public int SourceHeight { get; }

        private readonly IAVDecoderOptions _options;

        public StreamDecoder(IAvioStream inputStream, IAVDecoderOptions? options)
        {
            _inputStream = inputStream;
            _options = options ?? new AVDecoderOptions();

            _formatContext = ffmpeg.avformat_alloc_context();
            if (_formatContext == null)
            {
                throw new AVException("Cannot allocate FormatContext.");
            }

            // Discard frames marked as corrupted
            _formatContext->flags |= ffmpeg.AVFMT_FLAG_DISCARD_CORRUPT;

            var avioBuffer = (byte*)ffmpeg.av_malloc((ulong)_inputStream.ReadBufferLength);

            _formatContext->pb = ffmpeg.avio_alloc_context(
                avioBuffer, _inputStream.ReadBufferLength, 0, null,
                (avio_alloc_context_read_packet)_inputStream.Read, null,
                (avio_alloc_context_seek)_inputStream.Seek);
            if (_formatContext->pb == null)
            {
                throw new AVException("Cannot allocate AVIOContext.");
            }

            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                ffmpeg.avformat_open_input(formatContext, null, null, null)
                    .ThrowExceptionIfError();
            }

            ffmpeg.avformat_find_stream_info(_formatContext, null)
                .ThrowExceptionIfError();

            AVCodec* codec = null;
            _streamIndex = ffmpeg.av_find_best_stream(_formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0)
                .ThrowExceptionIfError();

            _codecContext = ffmpeg.avcodec_alloc_context3(codec);
            if (_codecContext == null)
            {
                throw new AVException("Cannot allocate codec context.");
            }

            // Copy stream parameters
            ffmpeg.avcodec_parameters_to_context(_codecContext, _formatContext->streams[_streamIndex]->codecpar)
                .ThrowExceptionIfError();

            ffmpeg.avcodec_open2(_codecContext, codec, null)
                .ThrowExceptionIfError();

            SourceWidth = _codecContext->width;
            SourceHeight = _codecContext->height;
        }

        private bool TryDecodeNextFrame()
        {
            ffmpeg.av_frame_unref(_frame);

            int error;
            do
            {
                do
                {
                    ffmpeg.av_packet_unref(_packet);

                    error = ffmpeg.av_read_frame(_formatContext, _packet);
                    if (error == ffmpeg.AVERROR_EOF)
                    {
                        return false;
                    }

                    error.ThrowExceptionIfError();
                }
                while (_packet->stream_index != _streamIndex);

                ffmpeg.avcodec_send_packet(_codecContext, _packet).ThrowExceptionIfError();

                error = ffmpeg.avcodec_receive_frame(_codecContext, _frame);
            }
            while (error == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            error.ThrowExceptionIfError();

            return true;
        }

        private void InitPacketAndFrame()
        {
            if (_packet == null && _frame == null)
            {
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
            else
            {
                ffmpeg.av_packet_unref(_packet);
                ffmpeg.av_frame_unref(_frame);
            }
        }

        public AVFrame* DecodeFrame()
        {
            InitPacketAndFrame();

            // Processing frame with black frame filter
            if (_options.EnableBlackFrameFilter &&
                _codecContext->codec_id != AVCodecID.AV_CODEC_ID_MJPEG) // mp3 covers
            {
                // Init filters
                if (_frameFilter == null)
                {
                    _frameFilter = new FrameFilter(_codecContext, _options);
                }

                AVDictionaryEntry* blackEntry = null;

                int decodedFramesCounter = 0;
                do
                {
                    if (!TryDecodeNextFrame())
                    {
                        break;
                    }

                    _frameFilter.ApplyFilters(_frame);

                    blackEntry = ffmpeg.av_dict_get(_frame->metadata, "lavfi.blackframe.pblack", null, 0);

                    decodedFramesCounter++;
                }
                while (blackEntry != null && decodedFramesCounter <= _options.BlackFramesLimit);

                return _frame;
            }

            // Just decode first frame
            TryDecodeNextFrame();

            return _frame;
        }

        public void Dispose()
        {
            fixed (AVFrame** frame = &_frame)
            {
                ffmpeg.av_frame_free(frame);
            }

            fixed (AVPacket** packet = &_packet)
            {
                ffmpeg.av_packet_free(packet);
            }

            fixed (AVCodecContext** codecContext = &_codecContext)
            {
                ffmpeg.avcodec_free_context(codecContext);
            }

            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                ffmpeg.avformat_close_input(formatContext);
            }

            _frameFilter?.Dispose();
        }
    }
}