namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class AVDecoderOptions : IAVDecoderOptions
    {
        public BlackFrameFilterOptions? BlackFilterOptions { get; set; }

        public FrameSizeOptions? FrameSizeOptions { get; set; }
    }
}