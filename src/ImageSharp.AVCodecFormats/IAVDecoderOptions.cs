namespace HeyRed.ImageSharp.AVCodecFormats
{
    public interface IAVDecoderOptions
    {
        /// <summary>
        /// Detect frames that are (almost) completely black.
        /// Default true.
        /// </summary>
        BlackFrameFilterOptions? BlackFilterOptions { get; }
    }
}