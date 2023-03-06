using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv;

public sealed class WmvConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the wmv format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(WmvFormat.Instance, AVDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new WmvFormatDetector());
    }
}