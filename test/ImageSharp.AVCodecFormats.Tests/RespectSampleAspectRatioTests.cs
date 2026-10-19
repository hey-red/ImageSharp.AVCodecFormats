using System.IO;
using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using SixLabors.ImageSharp.Formats;
using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class RespectSampleAspectRatioTests
{
    private readonly string _testVideoDataPath = TestHelpers.GetTestDataPath();

    [Theory]
    [InlineData("karina.mp4", 1214, 2160)]
    public void BasicTest(string fileName, int width, int height)
    {
        var decoderOptions = new AVDecoderOptions
        {
            GeneralOptions = new DecoderOptions
            {
                MaxFrames = 1
            },
            RespectSampleAspectRatio = true,
        };

        var filePath = Path.Combine(_testVideoDataPath, fileName);

        using var inputStream = File.OpenRead(filePath);
        using var image = Mp4Decoder.Instance.Decode(decoderOptions, inputStream);

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
    }
}