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

        public static bool IsMp4OrMovHeader(ReadOnlySpan<byte> headerSpan)
        {
            return
                // Offset 4
                headerSpan[0] == 0x66 &&    // f
                headerSpan[1] == 0x74 &&    // t
                headerSpan[2] == 0x79 &&    // y
                headerSpan[3] == 0x70;      // p
        }
    }
}