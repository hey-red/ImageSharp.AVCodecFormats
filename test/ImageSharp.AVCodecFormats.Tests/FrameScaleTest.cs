using System.IO;

using HeyRed.ImageSharp.AVCodecFormats;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;

using SixLabors.ImageSharp;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests
{
    public class FrameScaleTest
    {
        private readonly string _testVideoDataPath;

        private readonly Configuration _configuration;

        public FrameScaleTest()
        {
            _testVideoDataPath = TestUtils.GetTestDataPath();
            _configuration = TestUtils.GetImageSharpConfiguration();
        }

        [Fact]
        public void BasicTest()
        {
            string filePath = Path.Combine(_testVideoDataPath, "avc.mp4");

            var options = new AVDecoderOptions
            {
                TargetFrameSize = new Size(100, 100),
            };

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder(options));

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load(_configuration, inputStream);

            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }
    }
}