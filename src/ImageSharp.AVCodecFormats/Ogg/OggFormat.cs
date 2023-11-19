using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Ogg;

public sealed class OggFormat : IImageFormat<AVMetadata>
{
    private OggFormat()
    {
    }

    public static OggFormat Instance { get; } = new();

    public string Name => "OGG";

    public string DefaultMimeType => "application/ogg";

    public IEnumerable<string> MimeTypes => OggConstants.MimeTypes;

    public IEnumerable<string> FileExtensions => OggConstants.FileExtensions;

    public AVMetadata CreateDefaultFormatMetadata() => new();
}