using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    public sealed class Mp4ConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new Mp4FormatDetector());
        }
    }
}