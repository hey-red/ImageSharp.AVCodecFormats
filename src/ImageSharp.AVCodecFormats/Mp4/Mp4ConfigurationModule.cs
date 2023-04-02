using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

public sealed class Mp4ConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the mp4 format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, Mp4Decoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(Mp4Format.Instance, new Mp4Encoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new Mp4FormatDetector());
    }
}