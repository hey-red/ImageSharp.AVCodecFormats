using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3;

public sealed class Mp3Format : IImageFormat<AVMetadata>
{
    private Mp3Format()
    {
    }

    public static Mp3Format Instance { get; } = new();

    public string Name => "MP3";

    public string DefaultMimeType => "audio/mpeg";

    public IEnumerable<string> MimeTypes => Mp3Constants.MimeTypes;

    public IEnumerable<string> FileExtensions => Mp3Constants.FileExtensions;

    public AVMetadata CreateDefaultFormatMetadata() => new();
}