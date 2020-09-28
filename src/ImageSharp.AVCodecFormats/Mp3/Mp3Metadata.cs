using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3
{
    /// <summary>
    /// TODO: Base ID3 tags?
    /// </summary>
    public sealed class Mp3Metadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new Mp3Metadata();
    }
}