using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    /// <summary>
    /// Detects base mp4 container, such as
    /// mp4, mov, m4v, m4a, mp4a
    /// </summary>
    public sealed class Mp4FormatDetector : IImageFormatDetector
    {
        public int HeaderSize => 8;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? Mp4Format.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize)
            {
                return
                    header[4] == 0x66 &&    // f
                    header[5] == 0x74 &&    // t
                    header[6] == 0x79 &&    // y
                    header[7] == 0x70;      // p
            }

            return false;
        }
    }
}