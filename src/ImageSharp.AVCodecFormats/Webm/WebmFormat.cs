using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm;

public sealed class WebmFormat : IImageFormat<AVMetadata>
{
    private WebmFormat()
    {
    }

    public static WebmFormat Instance { get; } = new();

    public string Name => "WebM";

    public string DefaultMimeType => "video/webm";

    public IEnumerable<string> MimeTypes => WebmConstants.MimeTypes;

    public IEnumerable<string> FileExtensions => WebmConstants.FileExtensions;

    public AVMetadata CreateDefaultFormatMetadata() => new();
}