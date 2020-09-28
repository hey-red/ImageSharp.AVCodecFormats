using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov
{
    public sealed class MovMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new MovMetadata();
    }
}