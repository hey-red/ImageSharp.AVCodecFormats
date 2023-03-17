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
                MaxFrames = 1,
            },
            PreserveAspectRatio = aspectRatio
        };

        string filePath = Path.Combine(_testVideoDataPath, "mpeg4.avi");

        using var inputStream = File.OpenRead(filePath);
        using var image = AviDecoder.Instance.Decode(decoderOptions, inputStream);

        if (!aspectRatio)
        {
            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
        else
        {
            Assert.Equal(100, image.Width);
            Assert.Equal(56, image.Height);
        }
    }
}