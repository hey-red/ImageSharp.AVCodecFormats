using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Ogg;

public sealed class OggConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the ogg format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(OggFormat.Instance, OggDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(OggFormat.Instance, new OggEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new OggFormatDetector());
    }
}