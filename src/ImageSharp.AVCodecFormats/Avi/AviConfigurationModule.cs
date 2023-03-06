using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi;

public sealed class AviConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the avi format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(AviFormat.Instance, AVDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new AviFormatDetector());
    }
}