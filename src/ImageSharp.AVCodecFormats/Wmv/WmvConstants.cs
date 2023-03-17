using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv;

internal static class WmvConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "wmv" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/x-ms-wmv", };

    public static readonly byte[] AsfFormatHeader =
    {
        0x30,
        0x26,
        0xB2,
        0x75,
        0x8E,
        0x66,
        0xCF,
        0x11,
        0xA6,
        0xD9,
        0x00,
        0xAA,
        0x00,
        0x62,
        0xCE,
        0x6C
    };
}