using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi
{
    public sealed class AviConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(AviFormat.Instance, new AviDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new AviFormatDetector());
        }
    }
}