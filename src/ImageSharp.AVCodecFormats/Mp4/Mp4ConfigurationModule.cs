using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    public sealed class Mp4ConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public Mp4ConfigurationModule()
        {
        }

        public Mp4ConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, _decoderOptions == null ?
                new Mp4Decoder() :
                new Mp4Decoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new Mp4FormatDetector());
        }
    }
}