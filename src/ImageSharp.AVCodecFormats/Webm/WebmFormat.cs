using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm
{
    public sealed class WebmFormat : IImageFormat<WebmMetadata>
    {
        private WebmFormat()
        {
        }

        public static WebmFormat Instance { get; } = new();

        public string Name => "WebM";

        public string DefaultMimeType => "video/webm";

        public IEnumerable<string> MimeTypes => new[] { "video/webm", "audio/webm", };

        public IEnumerable<string> FileExtensions => new[] { "webm" };

        public WebmMetadata CreateDefaultFormatMetadata() => new();
    }
}