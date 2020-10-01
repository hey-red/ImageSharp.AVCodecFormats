using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests
{
    public class OptionsTests
    {
        private readonly string _testVideoDataPath;

        private readonly Configuration _configuration;

        public OptionsTests()
        {
            _testVideoDataPath = TestUtils.GetTestDataPath();
            _configuration = TestUtils.GetImageSharpConfiguration();
        }

        [Fact]
        public void BlackFrameFilterEnabledTest()
        {
            string filePath = Path.Combine(_testVideoDataPath, "black_frame.mp4");

            var options = new AVDecoderOptions { EnableBlackFrameFilter = true };

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder(options));

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load<Rgba32>(_configuration, inputStream);

            Assert.NotEqual(Color.Black.ToPixel<Rgba32>(), image[0, 0]);
        }

        [Fact]
        public void BlackFrameFilterDisabledTest()
        {
            string filePath = Path.Combine(_testVideoDataPath, "black_frame.mp4");

            var options = new AVDecoderOptions { EnableBlackFrameFilter = false };

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder(options));

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load<Rgba32>(_configuration, inputStream);

            Assert.Equal(Color.Black.ToPixel<Rgba32>(), image[0, 0]);
        }
    }
}