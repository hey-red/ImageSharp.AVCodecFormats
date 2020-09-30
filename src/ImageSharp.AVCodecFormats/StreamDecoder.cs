using System;
using System.IO;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class StreamDecoder : IDisposable
    {
        private const int AVIO_CTX_BUFFER_SIZE = 4096;

        private readonly Stream _inputStream;

        private readonly AVCodecContext* _codecContext;

        private readonly AVFormatContext* _formatContext;

        private readonly int _streamIndex;

        private AVFrame* _frame;

        private AVPacket* _packet;

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

        private readonly IAVDecoderOptions _options;

        public StreamDecoder(Stream inputStream, IAVDecoderOptions? options)
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

            var avioBuffer = (byte*)ffmpeg.av_malloc(AVIO_CTX_BUFFER_SIZE);

            _formatContext->pb = ffmpeg.avio_alloc_context(
                avioBuffer, AVIO_CTX_BUFFER_SIZE, 0, null,
                (avio_alloc_context_read_packet)ReadPacket, null,
                (avio_alloc_context_seek)Seek);
            if (_formatContext->pb == null)
            {
                throw new AVException("Cannot allocate AVIOContext.");
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
                throw new AVException("Cannot allocate codec context.");
            }

            // Copy stream parameters
            ffmpeg.avcodec_parameters_to_context(_codecContext, _formatContext->streams[_streamIndex]->codecpar);

            ffmpeg.avcodec_open2(_codecContext, codec, null)
                .ThrowExceptionIfError();

            SourceWidth = _codecContext->width;
            SourceHeight = _codecContext->height;
        }

        private AVFilterGraph* _filterGraph = null;

        private readonly AVFilterContext* _buffersrcFilterCtx;
        private readonly AVFilterContext* _buffersinkFilterCtx;
        private readonly AVFilterContext* _blackFrameFilterCtx;

        private void InitBlackFrameFilter()
        {
            if (_filterGraph != null) return;

            _filterGraph = ffmpeg.avfilter_graph_alloc();
            if (_filterGraph == null)
            {
                throw new AVException("Cannot allocate filter graph.");
            }

            fixed (AVFilterContext** buffersrcCtx = &_buffersrcFilterCtx)
            {
                // time_base and pixel_aspect here is incorrect, so set dummy values
                string srcArgs = $"video_size={_codecContext->width}x{_codecContext->height}:pix_fmt={(int)_codecContext->pix_fmt}:time_base=1/25:pixel_aspect=1/1";

                ffmpeg.avfilter_graph_create_filter(buffersrcCtx, ffmpeg.avfilter_get_by_name("buffer"), "in", srcArgs, null, _filterGraph)
                    .ThrowExceptionIfError();
            }

            fixed (AVFilterContext** buffersinkCtx = &_buffersinkFilterCtx)
            {
                ffmpeg.avfilter_graph_create_filter(buffersinkCtx, ffmpeg.avfilter_get_by_name("buffersink"), "out", null, null, _filterGraph)
                    .ThrowExceptionIfError();
            }

            // Configure black frame filter
            string args = $"amount={_options.BlackFrameAmount}:threshold={_options.BlackFrameThreshold}";

            // Create blackframe filter
            fixed (AVFilterContext** blackFrameCtx = &_blackFrameFilterCtx)
            {
                ffmpeg.avfilter_graph_create_filter(blackFrameCtx, ffmpeg.avfilter_get_by_name("blackframe"), null, args, null, _filterGraph)
                    .ThrowExceptionIfError();
            }

            // Link filters
            ffmpeg.avfilter_link(_buffersrcFilterCtx, 0, _blackFrameFilterCtx, 0)
                .ThrowExceptionIfError();

            ffmpeg.avfilter_link(_blackFrameFilterCtx, 0, _buffersinkFilterCtx, 0)
                .ThrowExceptionIfError();

            ffmpeg.avfilter_graph_config(_filterGraph, null)
                .ThrowExceptionIfError();
        }

        private void InitPackeAndFrame()
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

        public AVFrame* DecodeFrame()
        {
            InitPackeAndFrame();

            // Processing frame with black frame filter
            if (_options.EnableBlackFrameFilter)
            {
                InitBlackFrameFilter();

                AVDictionaryEntry* blackEntry = null;
                int decodedFramesCounter = 0;
                do
                {
                    if (!TryDecodeNextFrame())
                    {
                        break;
                    }

                    // Apply filters
                    ffmpeg.av_buffersrc_add_frame(_buffersrcFilterCtx, _frame)
                        .ThrowExceptionIfError();

                    ffmpeg.av_buffersink_get_frame(_buffersinkFilterCtx, _frame)
                        .ThrowExceptionIfError();

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

            fixed (AVFilterGraph** filterGraph = &_filterGraph)
            {
                ffmpeg.avfilter_graph_free(filterGraph);
            }
        }
    }
}