using HeyRed.ImageSharp.AVCodecFormats.Avi;
using HeyRed.ImageSharp.AVCodecFormats.Mkv;
using HeyRed.ImageSharp.AVCodecFormats.Mov;
using HeyRed.ImageSharp.AVCodecFormats.Mp3;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using HeyRed.ImageSharp.AVCodecFormats.MpegTs;
using HeyRed.ImageSharp.AVCodecFormats.Ogg;
using HeyRed.ImageSharp.AVCodecFormats.Webm;
using HeyRed.ImageSharp.AVCodecFormats.Wmv;

using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats;

public static class ConfigurationExtensions
{
    public static Configuration WithAVDecoders(this Configuration configuration)
    {
        configuration.Configure(new AviConfigurationModule());
        configuration.Configure(new MkvConfigurationModule());
        configuration.Configure(new MovConfigurationModule());
        configuration.Configure(new Mp4ConfigurationModule());
        configuration.Configure(new WebmConfigurationModule());
        configuration.Configure(new WmvConfigurationModule());
        configuration.Configure(new MpegTsConfigurationModule());
        configuration.Configure(new Mp3ConfigurationModule());
        configuration.Configure(new OggConfigurationModule());

        return configuration;
    }
}