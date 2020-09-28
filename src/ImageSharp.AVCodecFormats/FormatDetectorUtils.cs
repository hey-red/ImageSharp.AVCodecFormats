using System;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal static class FormatDetectorUtils
    {
        // TODO: should contains video
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