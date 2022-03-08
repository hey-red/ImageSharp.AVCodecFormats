using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class FrameSizeOptions
    {
        /// <summary>
        /// Rescale decoded frame with given size.
        /// </summary>
        public Size TargetFrameSize { get; set; }

        public bool PreserveAspectRatio { get; set; } = false;
    }
}