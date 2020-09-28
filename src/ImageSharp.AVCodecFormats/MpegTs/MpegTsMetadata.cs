using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs
{
    public sealed class MpegTsMetadata : IDeepCloneable
    {
        public IDeepCloneable DeepClone() => new MpegTsMetadata();
    }
}