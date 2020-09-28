# ImageSharp.AVCodecFormats
FFmpeg decoders for [ImageSharp](https://github.com/SixLabors/ImageSharp)

## Install
via [NuGet](https://www.nuget.org/packages/ImageSharp.AVCodecFormats):
```
PM> Install-Package ImageSharp.AVCodecFormats
```
Native libs for **x64** Linux and Windows:
```
PM> Install-Package ImageSharp.AVCodecFormats.Native
```
also, we can install separate native packages:

```
PM> Install-Package ImageSharp.AVCodecFormats.Native.win-x64
PM> Install-Package ImageSharp.AVCodecFormats.Native.linux-x64
```

Without native packages you should provide your own shared FFmpeg build and set path to libs:

`ffmpeg.RootPath = "/path/to/native/binaries"`

On linux you can install FFmpeg(4.3.x) from your package manager, but I have no guarantees that it will work as expected.

## Usage

```C#
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// Create your custom configuration with required decoders:
var configuration = new Configuration(
    new AviConfigurationModule(),
    new MkvConfigurationModule(),
    new MovConfigurationModule(),
    new Mp4ConfigurationModule(),
    new WebmConfigurationModule(),
    new WmvConfigurationModule(),
    new MpegTsConfigurationModule(),
    new Mp3ConfigurationModule());

using var inputStream = File.OpenRead("/path/to/video.mp4");
using var outputStream = File.OpenWrite("/path/to/resized_image.jpeg");

// Pass it into image load method(for example):
using var image = Image.Load(configuration, inputStream);

image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2)); 

image.Save(outputStream);
```
More info <https://docs.sixlabors.com/articles/imagesharp/configuration.html>

## Supported formats
mp4, webm, avi, mkv, mov, ts, wmv, mp3(extract cover image).

## Supported codecs
[Native package](https://www.nuget.org/packages/ImageSharp.AVCodecFormats.Native) provides codecs listed below:

H263, H264, VP8, VP9, AV1, MPEG-4, MJPEG, MS MPEG4(v1,v2,v3), WMV(v1,v2,v3), VC-1, MPEG-1 Audio Layer 3.

## License
[MIT](LICENSE)
