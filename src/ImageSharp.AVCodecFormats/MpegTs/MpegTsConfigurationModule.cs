using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs
{
    public sealed class MpegTsConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public MpegTsConfigurationModule()
        {
        }

        public MpegTsConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MpegTsFormat.Instance, _decoderOptions == null ?
                new MpegTsDecoder() :
                new MpegTsDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new MpegTsFormatDetector());
        }
    }
}