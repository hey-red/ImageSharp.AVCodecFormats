using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov;

public sealed class MovFormat : IImageFormat<MovMetadata>
{
    private MovFormat()
    {
    }

    public static MovFormat Instance { get; } = new();

    public string Name => "QuickTime Movie";

    public string DefaultMimeType => "video/quicktime";

    public IEnumerable<string> MimeTypes => MovConstants.MimeTypes;

    public IEnumerable<string> FileExtensions => MovConstants.FileExtensions;

    public MovMetadata CreateDefaultFormatMetadata() => new();
}