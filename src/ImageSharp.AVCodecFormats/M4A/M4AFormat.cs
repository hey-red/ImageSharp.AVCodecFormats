using System.Collections.Generic;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.M4A;

public sealed class M4AFormat : IImageFormat<AVMetadata>
{
    private M4AFormat()
    {
    }

    public static M4AFormat Instance { get; } = new();

    public string Name => "M4A";

    public string DefaultMimeType => "audio/mp4";

    public IEnumerable<string> MimeTypes => Mp4Constants.MimeTypes;

    public IEnumerable<string> FileExtensions => Mp4Constants.FileExtensions;

    public AVMetadata CreateDefaultFormatMetadata() => new();
}