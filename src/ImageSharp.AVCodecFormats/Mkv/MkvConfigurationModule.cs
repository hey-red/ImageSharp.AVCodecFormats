using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv
{
    public sealed class MkvConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MkvFormat.Instance, new MkvDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new MkvFormatDetector());
        }
    }
}