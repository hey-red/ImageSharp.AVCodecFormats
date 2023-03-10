using System.IO;
using System.Linq;

using HeyRed.ImageSharp.AVCodecFormats;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using Xunit;
using Xunit.Abstractions;

namespace ImageSharp.AVCodecFormats.Tests;

public class BasicTests
{
    private readonly string _testVideoDataPath;

    private readonly ITestOutputHelper _output;

    private readonly DecoderOptions _decoderOptions;

    private readonly ResizeOptions _resizeOptions;

    public BasicTests(ITestOutputHelper output)
    {
        _output = output;
        _testVideoDataPath = TestHelpers.GetTestDataPath();

        _decoderOptions = new DecoderOptions
        {
            MaxFrames = 1,
            Configuration = new Configuration().WithAVDecoders()
        };

        _resizeOptions = new ResizeOptions
        {
            Size = new Size(200),
            Mode = ResizeMode.Max
        };
    }

    private IImageFormat GetFormat(string format)
        => _decoderOptions.Configuration.ImageFormats
        .FirstOrDefault(x => x.FileExtensions.Contains(format));

    [Theory]
    [InlineData("avc.mp4", 640, 360)]
    [InlineData("av1.mp4", 640, 360)]
    [InlineData("avc.mkv", 640, 360)]
    [InlineData("mpeg.ts", 640, 360)]
    [InlineData("avc.mov", 640, 360)]
    [InlineData("mpeg4.avi", 640, 360)]
    [InlineData("mpeg4.wmv", 640, 360)]
    [InlineData("vp9.webm", 1280, 720)]
    [InlineData("vp8.webm", 344, 360)]
    [InlineData("mpga.mp3", 700, 700)]
    [InlineData("two_video_stream.webm", 640, 480)]
    public void IdentifyTests(string fileName, int width, int height)
    {
        string filePath = Path.Combine(_testVideoDataPath, fileName);
        string extension = Path.GetExtension(filePath).Replace(".", "");

        IImageFormat format = GetFormat(extension);

        _output.WriteLine($"Processing file: \"{Path.GetFileName(filePath)}\"");

        using var inputStream = File.OpenRead(filePath);
        ImageInfo imageInfo = Image.Identify(_decoderOptions, inputStream);

        _output.WriteLine($"Dimensions: {imageInfo.Width}x{imageInfo.Height}");

        Assert.Equal(width, imageInfo.Width);
        Assert.Equal(height, imageInfo.Height);
        Assert.Equal(format, imageInfo.Metadata.DecodedImageFormat);
    }

    [Theory]
    [InlineData("avc.mp4", 640, 360, 200, 112)]
    [InlineData("av1.mp4", 640, 360, 200, 112)]
    [InlineData("avc.mkv", 640, 360, 200, 112)]
    [InlineData("mpeg.ts", 640, 360, 200, 112)]
    [InlineData("avc.mov", 640, 360, 200, 112)]
    [InlineData("mpeg4.avi", 640, 360, 200, 112)]
    [InlineData("mpeg4.wmv", 640, 360, 200, 112)]
    [InlineData("vp9.webm", 1280, 720, 200, 112)]
    [InlineData("vp8.webm", 344, 360, 191, 200)]
    [InlineData("mpga.mp3", 700, 700, 200, 200)]
    public void ThumbnailTests(
        string fileName,
        int width, int height,
        int thumbWidth, int thumbHeight)
    {
        string filePath = Path.Combine(_testVideoDataPath, fileName);

        _output.WriteLine($"Processing file: \"{Path.GetFileName(filePath)}\"");

        using var inputStream = File.OpenRead(filePath);
        using var outputStream = new MemoryStream();
        using var image = Image.Load(_decoderOptions, inputStream);

        _output.WriteLine($"Source dimensions: {image.Width}x{image.Height}");

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);

        image.Mutate(x => x.Resize(_resizeOptions));

        _output.WriteLine($"Thumbnail dimensions: {image.Width}x{image.Height}");

        Assert.Equal(thumbWidth, image.Width);
        Assert.Equal(thumbHeight, image.Height);

        image.Save(outputStream, new JpegEncoder());

        Assert.NotEqual(0, outputStream.Length);
    }
}