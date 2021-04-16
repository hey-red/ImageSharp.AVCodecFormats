using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv
{
    public sealed class WmvFormat : IImageFormat<WmvMetadata>
    {
        private WmvFormat()
        {
        }

        public static WmvFormat Instance { get; } = new();

        public string Name => "Windows Media Video";

        public string DefaultMimeType => "video/x-ms-wmv";

        public IEnumerable<string> MimeTypes => new[] { "video/x-ms-wmv", };

        public IEnumerable<string> FileExtensions => new[] { "wmv" };

        public WmvMetadata CreateDefaultFormatMetadata() => new();
    }
}