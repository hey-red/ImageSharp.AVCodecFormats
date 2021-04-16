using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    public sealed class Mp4Format : IImageFormat<Mp4Metadata>
    {
        private Mp4Format()
        {
        }

        public static Mp4Format Instance { get; } = new();

        public string Name => "MP4";

        public string DefaultMimeType => "video/mp4";

        public IEnumerable<string> MimeTypes => new[] { "video/mp4", "application/mp4" };

        public IEnumerable<string> FileExtensions => new[] { "mp4" };

        public Mp4Metadata CreateDefaultFormatMetadata() => new();
    }
}