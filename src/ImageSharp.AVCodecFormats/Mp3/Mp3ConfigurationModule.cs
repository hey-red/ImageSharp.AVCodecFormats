using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3
{
    public sealed class Mp3ConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public Mp3ConfigurationModule()
        {
        }

        public Mp3ConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(Mp3Format.Instance, _decoderOptions == null ?
                new Mp3Decoder() :
                new Mp3Decoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new Mp3FormatDetector());
        }
    }
}