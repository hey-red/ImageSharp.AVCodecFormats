using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv
{
    public sealed class WmvConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public WmvConfigurationModule()
        {
        }

        public WmvConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(WmvFormat.Instance, _decoderOptions == null ?
                new WmvDecoder() :
                new WmvDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new WmvFormatDetector());
        }
    }
}