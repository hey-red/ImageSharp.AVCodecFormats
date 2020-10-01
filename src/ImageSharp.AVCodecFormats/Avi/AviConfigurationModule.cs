using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi
{
    public sealed class AviConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public AviConfigurationModule()
        {
        }

        public AviConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(AviFormat.Instance, _decoderOptions == null ?
                new AviDecoder() :
                new AviDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new AviFormatDetector());
        }
    }
}