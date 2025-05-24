using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class FrameFilterTests
{
    private readonly string _testVideoDataPath;

    public FrameFilterTests()
    {
        _testVideoDataPath = TestHelpers.GetTestDataPath();
    }

    /// <summary>
    ///     Adapted from https://github.com/FFmpeg/FFmpeg/blob/master/libavfilter/vf_blackframe.c
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="frameNum"></param>
    /// <returns></returns>
    private static bool BlackFrameFilter(ImageFrame frame, uint frameNum)
    {
        var image = (ImageFrame<Rgba32>)frame;

        // The threshold below which a pixel value is considered black
        var threshold = 32;
        // The percentage of the pixels that have to be below the threshold
        // for the frame to be considered black
        var percents = 98;

        var blackPixelsCount = 0;
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);

                for (var x = 0; x < pixelRow.Length; x++)
                {
                    Rgba32 pixel = pixelRow[x];

                    var value = (pixel.R + pixel.G + pixel.B) / 3;
                    if (value < threshold)
                    {
                        blackPixelsCount++;
                    }
                }
            }
        });

        var blackPercent = blackPixelsCount * 100 / (frame.Width * frame.Height);

        return blackPercent >= percents;
    }

    [Theory]
    [InlineData("av1.mp4", 9)]
    [InlineData("black_frame.mp4", 8)]
    public void BlackFrameFilterTest(string fileName, int expectedFramesCount)
    {
        var decoderOptions = new AVDecoderOptions
        {
            GeneralOptions = new DecoderOptions
            {
                MaxFrames = 10
            },
            FrameFilter = BlackFrameFilter
        };

        var filePath = Path.Combine(_testVideoDataPath, fileName);

        using FileStream inputStream = File.OpenRead(filePath);
        using Image image = Mp4Decoder.Instance.Decode(decoderOptions, inputStream);

        Assert.Equal(expectedFramesCount, image.Frames.Count);
    }
}