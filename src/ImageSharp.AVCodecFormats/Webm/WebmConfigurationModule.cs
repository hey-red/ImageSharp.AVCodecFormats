using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm
{
    public sealed class WebmConfigurationModule : IConfigurationModule
    {
        private readonly IAVDecoderOptions? _decoderOptions;

        public WebmConfigurationModule()
        {
        }

        public WebmConfigurationModule(IAVDecoderOptions decoderOptions) => _decoderOptions = decoderOptions;

        public void Configure(Configuration configuration)
        {
            configuration.ImageFormatsManager.SetDecoder(WebmFormat.Instance, _decoderOptions == null ?
                new WebmDecoder() :
                new WebmDecoder(_decoderOptions));

            configuration.ImageFormatsManager.AddImageFormatDetector(new WebmFormatDetector());
        }
    }
}