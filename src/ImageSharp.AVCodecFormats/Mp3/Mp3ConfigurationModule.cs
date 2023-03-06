using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3;

public sealed class Mp3ConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the mp3 format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(Mp3Format.Instance, AVDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new Mp3FormatDetector());
    }
}