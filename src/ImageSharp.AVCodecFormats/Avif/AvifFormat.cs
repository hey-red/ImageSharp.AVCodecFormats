using System.Collections.Generic;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

public sealed class AvifFormat : IImageFormat<AVMetadata>
{
    private AvifFormat()
    {
    }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static AvifFormat Instance { get; } = new AvifFormat();

    public string Name => "AVIF";

    public string DefaultMimeType => "image/avif";

    public IEnumerable<string> FileExtensions => AvifConstants.FileExtensions;

    public IEnumerable<string> MimeTypes => AvifConstants.MimeTypes;

    public AVMetadata CreateDefaultFormatMetadata()
    {
        return new();
    }
}