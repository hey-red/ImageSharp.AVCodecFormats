using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using Xunit;
using Xunit.Abstractions;

namespace ImageSharp.AVCodecFormats.Tests
{
    public class BasicTests
    {
        private readonly string _testVideoDataPath;

        private readonly ITestOutputHelper _output;

        private readonly Configuration _configuration;

        private readonly ResizeOptions _resizeOptions;

        public BasicTests(ITestOutputHelper output)
        {
            _output = output;
            _testVideoDataPath = TestUtils.GetTestDataPath();
            _configuration = TestUtils.GetImageSharpConfiguration();
            _resizeOptions = new ResizeOptions
            {
                Size = new Size(200),
                Mode = ResizeMode.Max
            };
        }

        [Theory]
        [InlineData("avc.mp4", 640, 360)]
        [InlineData("av1.mp4", 640, 360)]
        [InlineData("avc.mkv", 640, 360)]
        [InlineData("mpeg.ts", 640, 360)]
        [InlineData("avc.mov", 640, 360)]
        [InlineData("mpeg4.avi", 640, 360)]
        [InlineData("mpeg4.wmv", 640, 360)]
        [InlineData("vp9.webm", 1280, 720)]
        [InlineData("vp8.webm", 344, 360)]
        [InlineData("mpga.mp3", 700, 700)]
        [InlineData("two_video_stream.webm", 640, 480)]
        public void IdentifyVideoTest(string fileName, int width, int height)
        {
            string filePath = Path.Combine(_testVideoDataPath, fileName);

            _output.WriteLine($"Processing file: \"{Path.GetFileName(filePath)}\"");

            using var inputStream = File.OpenRead(filePath);
            IImageInfo imageInfo = Image.Identify(_configuration, inputStream);

            Assert.NotNull(imageInfo);

            _output.WriteLine($"Dimensions: {imageInfo.Width}x{imageInfo.Height}");

            Assert.Equal(width, imageInfo.Width);
            Assert.Equal(height, imageInfo.Height);
        }

        [Theory]
        [InlineData("avc.mp4", 640, 360, 200, 112)]
        [InlineData("av1.mp4", 640, 360, 200, 112)]
        [InlineData("avc.mkv", 640, 360, 200, 112)]
        [InlineData("mpeg.ts", 640, 360, 200, 112)]
        [InlineData("avc.mov", 640, 360, 200, 112)]
        [InlineData("mpeg4.avi", 640, 360, 200, 112)]
        [InlineData("mpeg4.wmv", 640, 360, 200, 112)]
        [InlineData("vp9.webm", 1280, 720, 200, 112)]
        [InlineData("vp8.webm", 344, 360, 191, 200)]
        [InlineData("mpga.mp3", 700, 700, 200, 200)]
        public void ThumbnailVideoTest(
            string fileName,
            int width, int height,
            int thumbWidth, int thumbHeight)
        {
            string filePath = Path.Combine(_testVideoDataPath, fileName);

            _output.WriteLine($"Processing file: \"{Path.GetFileName(filePath)}\"");

            using var inputStream = File.OpenRead(filePath);
            using var outputStream = new MemoryStream();
            using var image = Image.Load(_configuration, inputStream);

            _output.WriteLine($"Source dimensions: {image.Width}x{image.Height}");

            Assert.Equal(width, image.Width);
            Assert.Equal(height, image.Height);

            image.Mutate(x => x.Resize(_resizeOptions));

            _output.WriteLine($"Thumbnail dimensions: {image.Width}x{image.Height}");

            Assert.Equal(thumbWidth, image.Width);
            Assert.Equal(thumbHeight, image.Height);

            image.Save(outputStream, new JpegEncoder());

            Assert.NotEqual(0, outputStream.Length);
        }
    }
}