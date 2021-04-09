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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BasicTest(bool aspectRatio)
        {
            string filePath = Path.Combine(_testVideoDataPath, "avc.mp4");

            var options = new AVDecoderOptions
            {
                FrameSizeOptions = new FrameSizeOptions()
                {
                    TargetFrameSize = new Size(100, 100),
                    PreserveAspectRation = aspectRatio,
                },
            };

            _configuration.ImageFormatsManager.SetDecoder(Mp4Format.Instance, new Mp4Decoder(options));

            using var inputStream = File.OpenRead(filePath);
            using var image = Image.Load(_configuration, inputStream);

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
}