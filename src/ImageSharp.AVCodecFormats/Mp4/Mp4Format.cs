using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

public sealed class Mp4Format : IImageFormat<Mp4Metadata>
{
    private Mp4Format()
    {
    }

    public static Mp4Format Instance { get; } = new();

    public string Name => "MP4";

    public string DefaultMimeType => "video/mp4";

    public IEnumerable<string> MimeTypes => Mp4Constants.MimeTypes;

    public IEnumerable<string> FileExtensions => Mp4Constants.FileExtensions;

    public Mp4Metadata CreateDefaultFormatMetadata() => new();
}