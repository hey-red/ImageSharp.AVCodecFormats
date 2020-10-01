using System;

namespace HeyRed.ImageSharp.AVCodecFormats.Common
{
    internal static class FormatDetectorUtils
    {
        public static bool IsMkvOrWebmHeader(ReadOnlySpan<byte> headerSpan)
        {
            return
                headerSpan[0] == 0x1A &&
                headerSpan[1] == 0x45 &&
                headerSpan[2] == 0xDF &&
                headerSpan[3] == 0xA3;
        }
    }
}