using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;

using SixLabors.ImageSharp;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class FramesCountTests
{
    private readonly string _testVideoDataPath;

    public FramesCountTests()
    {
        _testVideoDataPath = TestHelpers.GetTestDataPath();
    }

    [Theory]
    [InlineData("avc.mp4", 400)]
    [InlineData("black_frame.mp4", 600)]
    public void TestFramesCounter(string fileName, int framesCount)
    {
        var filePath = Path.Combine(_testVideoDataPath, fileName);

        using FileStream inputStream = File.OpenRead(filePath);
        using Image image = Mp4Decoder.Instance.Decode(new AVDecoderOptions(), inputStream);

        Assert.Equal(framesCount, image.Frames.Count);
    }
}