using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

internal static class Mp4Constants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "mp4" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/mp4", "application/mp4" };

    public static readonly byte[] FtypMarker =
    {
        0x66,   // f
        0x74,   // t
        0x79,   // y
        0x70,   // p
    };
}
