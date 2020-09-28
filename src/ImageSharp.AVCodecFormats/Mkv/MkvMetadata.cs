using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv
{
    public sealed class MkvMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new MkvMetadata();
    }
}