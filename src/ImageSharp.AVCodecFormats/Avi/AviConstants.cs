using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi;

internal static class AviConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "avi" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/avi", "video/msvideo", "video/x-msvideo", };

    public static readonly byte[] RiffContainerHeader = 
    {
        0x52,   // R
        0x49,   // I
        0x46,   // F
        0x46,   // F
    };

    public static readonly byte[] AviListHeader =
    {
        0x41,   // A
        0x56,   // V
        0x49,   // I
        0x20,
        0x4C,   // L
        0x49,   // I
        0x53,   // S
        0x54,   // T
    };
}
