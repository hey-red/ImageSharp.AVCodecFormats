using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Metadata;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class ImageInfo : IImageInfo
    {
        public ImageInfo(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public PixelTypeInfo? PixelType { get; }

        public int Width { get; }

        public int Height { get; }

        public ImageMetadata? Metadata { get; }
    }
}