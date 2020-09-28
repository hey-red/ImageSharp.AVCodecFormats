using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov
{
    public sealed class MovConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MovFormat.Instance, new MovDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new MovFormatDetector());
        }
    }
}