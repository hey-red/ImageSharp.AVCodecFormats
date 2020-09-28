using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv
{
    public sealed class WmvFormatDetector : IImageFormatDetector
    {
        public int HeaderSize => 16;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? WmvFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize)
            {
                return
                    header[0] == 0x30 &&
                    header[1] == 0x26 &&
                    header[2] == 0xB2 &&
                    header[3] == 0x75 &&
                    header[4] == 0x8E &&
                    header[5] == 0x66 &&
                    header[6] == 0xCF &&
                    header[7] == 0x11 &&
                    header[8] == 0xA6 &&
                    header[9] == 0xD9 &&
                    header[10] == 0x00 &&
                    header[11] == 0xAA &&
                    header[12] == 0x00 &&
                    header[13] == 0x62 &&
                    header[14] == 0xCE &&
                    header[15] == 0x6C;
            }

            return false;
        }
    }
}