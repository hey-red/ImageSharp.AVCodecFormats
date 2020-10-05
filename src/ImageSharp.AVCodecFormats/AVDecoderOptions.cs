using System;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class AVDecoderOptions : IAVDecoderOptions
    {
        public bool EnableBlackFrameFilter { get; set; } = true;

        private int _blackFrameAmount = 98;

        public int BlackFrameAmount
        {
            get => _blackFrameAmount;
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Amount should be between 0 and 100.");
                }
                _blackFrameAmount = value;
            }
        }

        private int _blackFrameThreshold = 32;

        public int BlackFrameThreshold
        {
            get => _blackFrameThreshold;
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentException("Threshold should be between 0 and 255.");
                }
                _blackFrameThreshold = value;
            }
        }

        private int _blackFramesLimit = 5;

        public int BlackFramesLimit
        {
            get => _blackFramesLimit;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("FramesLimit should be greater or equal 0.");
                }
                _blackFramesLimit = value;
            }
        }
    }
}