using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs;

internal static class MpegTsConstants
{
    public const int G_MARK = 0x47; // G
    public static readonly IEnumerable<string> FileExtensions = new[] { "ts", "tsv" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/MP2T" };
}