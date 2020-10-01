using System;
using System.Text;

using HeyRed.ImageSharp.AVCodecFormats.Common;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv
{
    public sealed class MkvFormatDetector : IImageFormatDetector
    {
        private static readonly byte[] _matroskaMarker = Encoding.ASCII.GetBytes("matroska");

        public int HeaderSize => 39;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? MkvFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize &&
                FormatDetectorUtils.IsMkvOrWebmHeader(header))
            {
                return header.Slice(4).IndexOf(_matroskaMarker) > -1;
            }

            return false;
        }
    }
}