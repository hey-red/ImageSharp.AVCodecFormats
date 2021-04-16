using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv
{
    public sealed class MkvFormat : IImageFormat<MkvMetadata>
    {
        private MkvFormat()
        {
        }

        public static MkvFormat Instance { get; } = new();

        public string Name => "Matroska";

        public string DefaultMimeType => "video/x-matroska";

        public IEnumerable<string> MimeTypes => new[] { "video/x-matroska", "audio/x-matroska", };

        public IEnumerable<string> FileExtensions => new[] { "mkv" };

        public MkvMetadata CreateDefaultFormatMetadata() => new();
    }
}