using System;
using System.Text;

using HeyRed.ImageSharp.AVCodecFormats.Common;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm
{
    public sealed class WebmFormatDetector : IImageFormatDetector
    {
        private static readonly byte[] _webmMarker = Encoding.ASCII.GetBytes("webm");

        public int HeaderSize => 39;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? WebmFormat.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize &&
                FormatDetectorUtils.IsMkvOrWebmHeader(header))
            {
                return header.Slice(4).IndexOf(_webmMarker) > -1;
            }

            return false;
        }
    }
}