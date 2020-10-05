using System;

using HeyRed.ImageSharp.AVCodecFormats.Common;

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
            if (header.Length >= HeaderSize &&
                FormatDetectorUtils.IsMp4OrMovHeader(header.Slice(4)))
            {
                return
                    header[8] == 0x71 &&    // q
                    header[9] == 0x74;      // t
            }

            return false;
        }
    }
}