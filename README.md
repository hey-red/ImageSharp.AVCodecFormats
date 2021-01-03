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

Without native packages you should provide your own shared FFmpeg build and set path:

`FFmpegBinaries.Path = "/path/to/native/binaries"`

On Linux you have another way to get native libs. Just install ffmpeg from your package manager, but I have no guarantees that it will work as expected.

## Usage

```C#
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using HeyRed.ImageSharp.AVCodecFormats;

// Create custom configuration with all available decoders
var configuration = new Configuration().WithAVDecoders(); // With options WithAVDecoders(options)

// Or only required decoders
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

// Pass it into image load method(for example)
using var image = Image.Load(configuration, inputStream);

// Resize
image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2)); 

// Save using jpeg encoder
image.Save(outputStream, new JpegEncoder());
```
More info <https://docs.sixlabors.com/articles/imagesharp/configuration.html>

## Blackframe filter
You can skip first (n) of frames that are black or almost black.
```C#
var options = new AVDecoderOptions
{
    // With default values(see docs)
    BlackFilterOptions = new BlackFrameFilterOptions()
};

var configuration = new Configuration().WithAVDecoders(options);

using var inputStream = File.OpenRead(filePath);
using var image = Image.Load(configuration, inputStream);
```
The docs for filter options can be found [here](https://github.com/hey-red/ImageSharp.AVCodecFormats/blob/master/src/ImageSharp.AVCodecFormats/BlackFrameFilterOptions.cs).

## Supported formats
mp4, webm, avi, mkv, mov, ts, wmv, mp3(extract cover image).

## Supported codecs
[Native package](https://www.nuget.org/packages/ImageSharp.AVCodecFormats.Native) provides codecs listed below:

H263, H264, VP8, VP9, AV1, MPEG-4, MJPEG, PNG, MS MPEG4(v1,v2,v3), WMV(v1,v2,v3), VC-1, MPEG-1 Audio Layer 3.

## License
[MIT](LICENSE)
