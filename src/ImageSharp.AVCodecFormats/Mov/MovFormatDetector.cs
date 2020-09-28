using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov
{
    /// <summary>
    /// TODO: mdat, wide etc.
    /// </summary>
    public sealed class MovFormatDetector : IImageFormatDetector
    {
        public int HeaderSize => 10;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? MovFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize)
            {
                return
                    header[4] == 0x66 &&    // f
                    header[5] == 0x74 &&    // t
                    header[6] == 0x79 &&    // y
                    header[7] == 0x70 &&    // p
                    header[8] == 0x71 &&    // q
                    header[9] == 0x74;      // t
            }

            return false;
        }
    }
}