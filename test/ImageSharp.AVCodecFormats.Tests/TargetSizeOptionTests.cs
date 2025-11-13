using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Avi;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class TargetSizeOptionTests
{
    private readonly string _testVideoDataPath;

    public TargetSizeOptionTests()
    {
        _testVideoDataPath = TestHelpers.GetTestDataPath();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void BasicTest(bool aspectRatio)
    {
        var decoderOptions = new AVDecoderOptions
        {
            GeneralOptions = new DecoderOptions
            {
                TargetSize = new Size(100),
                MaxFrames = 1
            },
            PreserveAspectRatio = aspectRatio
        };

        string filePath = Path.Combine(_testVideoDataPath, "mpeg4.avi");

        using FileStream inputStream = File.OpenRead(filePath);
        using Image image = AviDecoder.Instance.Decode(decoderOptions, inputStream);

        Assert.Equal(100, image.Width);
        Assert.Equal(!aspectRatio ? 100 : 56, image.Height);
    }
}