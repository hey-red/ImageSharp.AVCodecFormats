using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv
{
    public sealed class MkvConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public MkvConfigurationModule()
        {
        }

        public MkvConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(MkvFormat.Instance, _decoderOptions == null ?
                new MkvDecoder() :
                new MkvDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new MkvFormatDetector());
        }
    }
}