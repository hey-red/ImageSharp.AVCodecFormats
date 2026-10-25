using System;
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
    [InlineData(104, 58)] // control: 104 * 3 is a multiple of 4
    public void Rgb24_WidthNotMultipleOf4_DecodesWithExactSize(int width, int height)
        => AssertDecodes<Rgb24>(width, height);

    [Theory]
    [InlineData(101, 56)]
    [InlineData(102, 57)]
    [InlineData(103, 58)]
    [InlineData(104, 58)]
    public void Bgr24_WidthNotMultipleOf4_DecodesWithExactSize(int width, int height)
        => AssertDecodes<Bgr24>(width, height);

    // The dimension checks above cannot see a skewed picture. If the converter writes rows with a padded stride
    // into a tightly packed buffer, every row is shifted by up to 3 bytes more than the previous one, so the
    // colors no longer match the same frame decoded as Rgba32 (whose rows are always tightly packed).
    [Theory]
    [InlineData(101, 56)]
    [InlineData(102, 57)]
    [InlineData(103, 58)]
    [InlineData(104, 58)]
    public void Rgb24_PixelsMatchRgba32(int width, int height)
    {
        using Image<Rgba32> reference = Load<Rgba32>(width, height);
        using Image<Rgb24> image = Load<Rgb24>(width, height);

        AssertSameColors(reference, image, p => (p.R, p.G, p.B));
    }

    [Theory]
    [InlineData(101, 56)]
    [InlineData(102, 57)]
    [InlineData(103, 58)]
    [InlineData(104, 58)]
    public void Bgr24_PixelsMatchRgba32(int width, int height)
    {
        using Image<Rgba32> reference = Load<Rgba32>(width, height);
        using Image<Bgr24> image = Load<Bgr24>(width, height);

        AssertSameColors(reference, image, p => (p.R, p.G, p.B));
    }

    private void AssertDecodes<TPixel>(int width, int height)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using Image<TPixel> image = Load<TPixel>(width, height);

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
    }

    private Image<TPixel> Load<TPixel>(int width, int height)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        var options = new DecoderOptions
        {
            MaxFrames = 1,
            TargetSize = new Size(width, height),
            Configuration = new Configuration().WithAVDecoders()
        };

        using var stream = File.OpenRead(Path.Combine(_testVideoDataPath, "mpeg4.avi"));
        return Image.Load<TPixel>(options, stream);
    }

    private static void AssertSameColors<TPixel>(
        Image<Rgba32> reference,
        Image<TPixel> image,
        Func<TPixel, (byte R, byte G, byte B)> rgb)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        Assert.Equal(reference.Width, image.Width);
        Assert.Equal(reference.Height, image.Height);

        // Both images come from the same frame through the same color conversion, so they should be (almost)
        // identical. A skewed picture is off by tens of levels on average.
        long totalDifference = 0;
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                Rgba32 expected = reference[x, y];
                var actual = rgb(image[x, y]);

                totalDifference += Math.Abs(expected.R - actual.R)
                                   + Math.Abs(expected.G - actual.G)
                                   + Math.Abs(expected.B - actual.B);
            }
        }

        var meanDifference = totalDifference / (3.0 * image.Width * image.Height);
        Assert.True(meanDifference < 2.0, $"Mean color difference per channel is {meanDifference:F2}.");
    }
}