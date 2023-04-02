using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs;

public sealed class MpegTsConfigurationModule : IImageFormatConfigurationModule
{
    /// <summary>
    /// Registers the image encoders, decoders and mime type detectors for the MPEG-TS format.
    /// </summary>
    public void Configure(Configuration configuration)
    {
        configuration.ImageFormatsManager.SetDecoder(MpegTsFormat.Instance, MpegTsDecoder.Instance);
        configuration.ImageFormatsManager.SetEncoder(MpegTsFormat.Instance, new MpegTsEncoder());
        configuration.ImageFormatsManager.AddImageFormatDetector(new MpegTsFormatDetector());
    }
}