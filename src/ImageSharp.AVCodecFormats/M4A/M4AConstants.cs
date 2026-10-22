using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.M4A;

internal static class M4AConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "m4a" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "audio/mp4" };

    /// <summary>
    ///     ISO Media
    /// </summary>
    public static readonly byte[] FtypMarker =
    {
        0x66, // f
        0x74, // t
        0x79, // y
        0x70  // p
    };
    
    /// <summary>
    ///     M4A
    /// </summary>
    public static readonly byte[] M4ABrandHeader =
    {
        0x4D, // M
        0x34, // 4
        0x41, // A
        0x20
    };
}