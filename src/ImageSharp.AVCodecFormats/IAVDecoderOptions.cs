namespace HeyRed.ImageSharp.AVCodecFormats
{
    public interface IAVDecoderOptions
    {
        /// <summary>
        /// Detect frames that are (almost) completely black.
        /// More info: https://ffmpeg.org/ffmpeg-filters.html#blackframe
        /// Default true.
        /// </summary>
        bool EnableBlackFrameFilter { get; }

        /// <summary>
        /// The percentage of the pixels that have to be below the threshold.
        /// Default 98.
        /// </summary>
        int BlackFrameAmount { get; }

        /// <summary>
        /// The threshold below which a pixel value is considered black.
        /// Default 32.
        /// </summary>
        int BlackFrameThreshold { get; }

        /// <summary>
        /// Limit of frames that can be black.
        /// Decoder returns next frame if value is exceeded.
        /// Default 5.
        /// </summary>
        int BlackFramesLimit { get; }
    }
}