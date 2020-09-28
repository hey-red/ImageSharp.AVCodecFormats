using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm
{
    public sealed class WebmMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new WebmMetadata();
    }
}