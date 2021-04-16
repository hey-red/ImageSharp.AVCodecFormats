using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3
{
    public sealed class Mp3Format : IImageFormat<Mp3Metadata>
    {
        private Mp3Format()
        {
        }

        public static Mp3Format Instance { get; } = new();

        public string Name => "MP3";

        public string DefaultMimeType => "audio/mpeg";

        public IEnumerable<string> MimeTypes => new[] { "audio/mpeg", };

        public IEnumerable<string> FileExtensions => new[] { "mp3", "mpga" };

        public Mp3Metadata CreateDefaultFormatMetadata() => new();
    }
}