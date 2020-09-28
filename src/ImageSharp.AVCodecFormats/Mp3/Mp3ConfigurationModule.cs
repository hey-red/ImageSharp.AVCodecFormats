using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3
{
    public sealed class Mp3ConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(Mp3Format.Instance, new Mp3Decoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new Mp3FormatDetector());
        }
    }
}