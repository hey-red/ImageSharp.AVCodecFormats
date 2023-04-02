using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov;

public sealed class MovConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the mov format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(MovFormat.Instance, MovDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(MovFormat.Instance, new MovEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new MovFormatDetector());
    }
}