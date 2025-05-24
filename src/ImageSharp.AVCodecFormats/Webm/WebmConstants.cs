using System.Collections.Generic;
using System.Text;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm;

internal static class WebmConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "webm" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/webm" };

    public static readonly byte[] WebmContainerStart =
    {
        0x1A,
        0x45,
        0xDF,
        0xA3
    };

    public static readonly byte[] WebmMarker = Encoding.ASCII.GetBytes("webm");
}