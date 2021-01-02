using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests
{
    public class BlackFrameFilterTest
    {
        private readonly string _testVideoDataPath;

        private readonly Configuration _configuration;

        public BlackFrameFilterTest()
        {
            _testVideoDataPath = TestUtils.GetTestDataPath();
            _configuration = TestUtils.GetImageSharpConfiguration();
        }

        [Fact]
        public void BlackFrameFilterEnabledTest()
        {
            string filePath = Path.Combine(_testVideoDataPath, "black_frame.mp4");

            var options = new AVDecoderOptions
            {
                BlackFilterOptions = new BlackFrameFilterOptions()
            };

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder(options));

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load<Rgb24>(_configuration, inputStream);

            Assert.NotEqual(Color.Black.ToPixel<Rgb24>(), image[0, 0]);
        }

        [Fact]
        public void BlackFrameFilterDisabledTest()
        {
            string filePath = Path.Combine(_testVideoDataPath, "black_frame.mp4");

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder());

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load<Rgb24>(_configuration, inputStream);

            Assert.Equal(Color.Black.ToPixel<Rgb24>(), image[0, 0]);
        }
    }
}