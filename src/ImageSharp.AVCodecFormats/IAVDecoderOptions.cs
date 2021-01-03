namespace HeyRed.ImageSharp.AVCodecFormats
{
    public interface IAVDecoderOptions
    {
        /// <summary>
        /// Detect frames that are (almost) completely black.
        /// </summary>
        BlackFrameFilterOptions? BlackFilterOptions { get; }
    }
}