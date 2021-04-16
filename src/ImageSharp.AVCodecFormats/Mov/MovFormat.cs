using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov
{
    public sealed class MovFormat : IImageFormat<MovMetadata>
    {
        private MovFormat()
        {
        }

        public static MovFormat Instance { get; } = new();

        public string Name => "QuickTime Movie";

        public string DefaultMimeType => "video/quicktime";

        public IEnumerable<string> MimeTypes => new[] { "video/quicktime", };

        public IEnumerable<string> FileExtensions => new[] { "mov" };

        public MovMetadata CreateDefaultFormatMetadata() => new();
    }
}