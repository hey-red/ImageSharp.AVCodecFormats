using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi
{
    public sealed class AviFormat : IImageFormat<AviMetadata>
    {
        private AviFormat()
        {
        }

        public static AviFormat Instance { get; } = new();

        public string Name => "AVI";

        public string DefaultMimeType => "video/avi";

        public IEnumerable<string> MimeTypes => new[] { "video/avi", "video/msvideo", "video/x-msvideo", };

        public IEnumerable<string> FileExtensions => new[] { "avi" };

        public AviMetadata CreateDefaultFormatMetadata() => new();
    }
}