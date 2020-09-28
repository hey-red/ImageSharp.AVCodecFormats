using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs
{
    public sealed class MpegTsConfigurationModule : IConfigurationModule
    {
        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MpegTsFormat.Instance, new MpegTsDecoder());
            configuration.ImageFormatsManager.AddImageFormatDetector(new MpegTsFormatDetector());
        }
    }
}