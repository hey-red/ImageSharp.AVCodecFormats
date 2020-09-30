using System;

using FFmpeg.AutoGen;

using HeyRed.ImageSharp.AVCodecFormats.Common;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal sealed unsafe class FrameFilter : IDisposable
    {
        private readonly AVFilterGraph* _filterGraph;

        private readonly AVFilterContext* _buffersrcFilterCtx;

        private readonly AVFilterContext* _buffersinkFilterCtx;

        private readonly AVFilterContext* _blackFrameFilterCtx;

        public FrameFilter(AVCodecContext* codecContext, IAVDecoderOptions options)
        {
            _filterGraph = ffmpeg.avfilter_graph_alloc();
            if (_filterGraph == null)
            {
                throw new AVException("Cannot allocate filter graph.");
            }

            fixed (AVFilterContext** buffersrcCtx = &_buffersrcFilterCtx)
            {
                // time_base and pixel_aspect here is incorrect, so set dummy values
                string srcArgs =
                    $"video_size={codecContext->width}x{codecContext->height}:" +
                    $"pix_fmt={(int)codecContext->pix_fmt}:time_base=1/25:pixel_aspect=1/1";

                ffmpeg.avfilter_graph_create_filter(buffersrcCtx, ffmpeg.avfilter_get_by_name("buffer"), "in", srcArgs, null, _filterGraph)
                    .ThrowExceptionIfError();
            }

            fixed (AVFilterContext** buffersinkCtx = &_buffersinkFilterCtx)
            {
                ffmpeg.avfilter_graph_create_filter(buffersinkCtx, ffmpeg.avfilter_get_by_name("buffersink"), "out", null, null, _filterGraph)
                    .ThrowExceptionIfError();
            }

            // Configure black frame filter
            string args = $"amount={options.BlackFrameAmount}:threshold={options.BlackFrameThreshold}";

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

        public void ApplyFilters(AVFrame* frame)
        {
            ffmpeg.av_buffersrc_add_frame(_buffersrcFilterCtx, frame)
                .ThrowExceptionIfError();

            ffmpeg.av_buffersink_get_frame(_buffersinkFilterCtx, frame)
                .ThrowExceptionIfError();
        }

        public void Dispose()
        {
            fixed (AVFilterGraph** filterGraph = &_filterGraph)
            {
                ffmpeg.avfilter_graph_free(filterGraph);
            }
        }
    }
}