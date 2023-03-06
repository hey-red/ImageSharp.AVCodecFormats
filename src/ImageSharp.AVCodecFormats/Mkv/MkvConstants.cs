using System.Collections.Generic;
using System.Text;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv;

internal static class MkvConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "mkv" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/x-matroska" };

    public static readonly byte[] MkvContainerStart =
    {
        0x1A,
        0x45,
        0xDF,
        0xA3,
    };

    public static readonly byte[] MatroskaMarker = Encoding.ASCII.GetBytes("matroska");
}
