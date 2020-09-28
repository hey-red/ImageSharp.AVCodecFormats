using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv
{
    public sealed class WmvConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(WmvFormat.Instance, new WmvDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new WmvFormatDetector());
        }
    }
}