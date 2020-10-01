using System;
using System.Text;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3
{
    public sealed class Mp3FormatDetector : IImageFormatDetector
    {
        private static readonly byte[] _picMarker = Encoding.ASCII.GetBytes("PIC");

        public int HeaderSize => 1024;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? Mp3Format.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize &&
                header[0] == 0x49 &&    // I
                header[1] == 0x44 &&    // D
                header[2] == 0x33)      // 3
            {
                // Match PIC or APIC
                return header.Slice(3).IndexOf(_picMarker) > -1;
            }

            return false;
        }
    }
}