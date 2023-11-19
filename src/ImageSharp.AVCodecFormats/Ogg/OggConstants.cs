using System.Collections.Generic;
using System.Text;

namespace HeyRed.ImageSharp.AVCodecFormats.Ogg;

internal static class OggConstants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "ogg", "ogv", "oga", "ogx", "ogm", "spx", "opus" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/ogg", "audio/ogg", "application/ogg" };

    public static readonly byte[] OggMarker = Encoding.ASCII.GetBytes("OggS");
}
