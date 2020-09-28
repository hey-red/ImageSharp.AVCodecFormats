using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm
{
    public sealed class WebmConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(WebmFormat.Instance, new WebmDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new WebmFormatDetector());
        }
    }
}