using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov
{
    public sealed class MovConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public MovConfigurationModule()
        {
        }

        public MovConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MovFormat.Instance, _decoderOptions == null ?
                new MovDecoder() :
                new MovDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new MovFormatDetector());
        }
    }
}