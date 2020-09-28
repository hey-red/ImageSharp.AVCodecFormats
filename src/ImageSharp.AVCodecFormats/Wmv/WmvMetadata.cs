using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv
{
    public sealed class WmvMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new WmvMetadata();
    }
}