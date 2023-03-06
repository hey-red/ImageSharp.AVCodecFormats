using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm;

public sealed class WebmConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the webm format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(WebmFormat.Instance, AVDecoder.Instance);
        configuration.ImageFormatsManager.AddImageFormatDetector(new WebmFormatDetector());
    }
}