using FFMediaToolkit.Graphics;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal class BlackFrameFilter
    {
        private readonly BlackFrameFilterOptions _options;

        public BlackFrameFilter(BlackFrameFilterOptions options)
        {
            _options = options;
        }

        public bool IsBlackFrame(ImageData frame)
        {
            int numBlackPix = 0;

            for (int i = 0; i < frame.ImageSize.Height; i++)
            {
                for (int x = 0; x < frame.ImageSize.Width; x++)
                {
                    if (frame.Data[x] < _options.FrameThreshold)
                    {
                        numBlackPix++;
                    }
                }
            }

            int blackPercent = numBlackPix * 100 / (frame.ImageSize.Width * frame.ImageSize.Height);

            return blackPercent >= _options.FrameAmount;
        }
    }
}