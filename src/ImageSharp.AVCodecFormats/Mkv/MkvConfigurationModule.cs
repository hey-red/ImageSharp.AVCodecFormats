using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv;

public sealed class MkvConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the mkv format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(MkvFormat.Instance, MkvDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(MkvFormat.Instance, new MkvEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new MkvFormatDetector());
    }
}