using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov;

internal static class MovConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "mov" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/quicktime" };

    public static readonly byte[] FtypQtMarker =
    {
        0x66,   // f
        0x74,   // t
        0x79,   // y
        0x70,   // p
        0x71,   // q
        0x74,   // t
    };
}