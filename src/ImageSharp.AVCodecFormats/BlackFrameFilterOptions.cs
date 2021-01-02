using System;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class BlackFrameFilterOptions
    {
        private int _frameAmount = 98;

        /// <summary>
        /// The percentage of the pixels that have to be below the threshold.
        /// Default 98.
        /// </summary>
        public int FrameAmount
        {
            get => _frameAmount;
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Amount should be between 0 and 100.");
                }
                _frameAmount = value;
            }
        }

        private int _frameThreshold = 32;

        /// <summary>
        /// The threshold below which a pixel value is considered black.
        /// Value should be between 0 and 255. Default 32.
        /// </summary>
        public int FrameThreshold
        {
            get => _frameThreshold;
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentException("Threshold should be between 0 and 255.");
                }
                _frameThreshold = value;
            }
        }

        private int _framesLimit = 5;

        /// <summary>
        /// Limit of frames that can be black.
        /// Decoder returns next frame if value is exceeded.
        /// Value should be greater or equal 0. Default 5.
        /// </summary>
        public int FramesLimit
        {
            get => _framesLimit;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("FramesLimit should be greater or equal 0.");
                }
                _framesLimit = value;
            }
        }
    }
}