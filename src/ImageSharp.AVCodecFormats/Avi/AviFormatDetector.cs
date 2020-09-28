using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi
{
    public sealed class AviFormatDetector : IImageFormatDetector
    {
        public int HeaderSize => 16;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? AviFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize)
            {
                return
                    header[0] == 0x52 &&    // R
                    header[1] == 0x49 &&    // I
                    header[2] == 0x46 &&    // F
                    header[3] == 0x46 &&    // F

                    header[8] == 0x41 &&    // A
                    header[9] == 0x56 &&    // V
                    header[10] == 0x49 &&   // I
                    header[11] == 0x20 &&
                    header[12] == 0x4C &&   // L
                    header[13] == 0x49 &&   // I
                    header[14] == 0x53 &&   // S
                    header[15] == 0x54;     // T
            }

            return false;
        }
    }
}