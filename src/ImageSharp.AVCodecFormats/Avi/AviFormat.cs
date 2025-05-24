using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi;

public sealed class AviFormat : IImageFormat<AVMetadata>
{
    private AviFormat()
    {
    }

    /// <summary>
    ///     Gets the current instance.
    /// </summary>
    public static AviFormat Instance { get; } = new();

    public string Name => "AVI";

    public string DefaultMimeType => "video/avi";

    public IEnumerable<string> MimeTypes => AviConstants.MimeTypes;

    public IEnumerable<string> FileExtensions => AviConstants.FileExtensions;

    public AVMetadata CreateDefaultFormatMetadata() => new();
}