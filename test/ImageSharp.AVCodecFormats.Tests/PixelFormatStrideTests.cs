using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class PixelFormatStrideTests
{
    private readonly string _testVideoDataPath = TestHelpers.GetTestDataPath();

    [Theory]
    [InlineData(101, 56)]
    [InlineData(102, 57)]
    [InlineData(103, 58)]
    public void Rgb24_WidthNotMultipleOf4_DecodesWithExactSize(int width, int height)
        => AssertDecodes<Rgb24>(width, height);

    [Theory]
    [InlineData(101, 56)]
    [InlineData(102, 57)]
    [InlineData(103, 58)]
    public void Bgr24_WidthNotMultipleOf4_DecodesWithExactSize(int width, int height)
        => AssertDecodes<Bgr24>(width, height);

    private void AssertDecodes<TPixel>(int width, int height)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var options = new DecoderOptions
        {
            MaxFrames = 1,
            TargetSize = new Size(width, height),
            Configuration = new Configuration().WithAVDecoders()
        };

        using var stream = File.OpenRead(Path.Combine(_testVideoDataPath, "mpeg4.avi"));
        using Image<TPixel> image = Image.Load<TPixel>(options, stream);

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
    }
}