using System.Drawing;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class AVDecoderOptions : IAVDecoderOptions
    {
        public BlackFrameFilterOptions? BlackFilterOptions { get; set; }

        public Size? TargetFrameSize { get; set; }
    }
}