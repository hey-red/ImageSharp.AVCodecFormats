using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.M4A;

public sealed class M4AConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    ///     Registers the image encoders, decoders and mime type detectors for the m4a format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(M4AFormat.Instance, M4ADecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(M4AFormat.Instance, new M4AEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new M4AFormatDetector());
    }
}