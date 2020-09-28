using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs
{
    public sealed class MpegTsFormatDetector : IImageFormatDetector
    {
        private const int G_MARK = 0x47;

        public int HeaderSize => 189;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? MpegTsFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize)
            {
                // Every 188
                return
                    header[0] == G_MARK &&  // G
                    header[188] == G_MARK;  // G
            }

            return false;
        }
    }
}