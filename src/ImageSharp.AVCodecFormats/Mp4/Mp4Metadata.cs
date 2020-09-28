using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    public sealed class Mp4Metadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new Mp4Metadata();
    }
}