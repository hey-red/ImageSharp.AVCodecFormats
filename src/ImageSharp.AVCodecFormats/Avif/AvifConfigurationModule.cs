using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

public class AvifConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    ///     Registers the image encoders, decoders and mime type detectors for the avif format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(AvifFormat.Instance, AvifDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(AvifFormat.Instance, new AvifEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new AvifImageFormatDetector());
    }
}