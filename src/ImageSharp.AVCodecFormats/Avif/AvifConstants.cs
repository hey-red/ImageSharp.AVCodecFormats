using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

internal static class AvifConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "avif" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "image/avif" };
    
    public static readonly byte[] FtypHeader =
    {
        0x66,   // f
        0x74,   // t
        0x79,   // y
        0x70,   // p
    };

    public static readonly byte[] AvifBrandHeader =
    {
        0x61,   // a
        0x76,   // v
        0x69,   // i
        0x66,   // f
    };
    
    public static readonly byte[] AvisBrandHeader =
    {
        0x61,   // a
        0x76,   // v
        0x69,   // i
        0x73,   // s
    };
}