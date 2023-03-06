using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3;

internal static class Mp3Constants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "mp3", "mpga" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "audio/mpeg", };

    public static readonly byte[] Id3Marker =
    {
        0x49,   // I
        0x44,   // D
        0x33,   // 3
    };

    public static readonly byte[] PicMarker =
    {
        0x50,   // P
        0x49,   // I
        0x43,   // C
    };
}
