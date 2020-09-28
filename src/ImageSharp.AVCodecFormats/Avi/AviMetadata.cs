using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi
{
    public sealed class AviMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new AviMetadata();
    }
}