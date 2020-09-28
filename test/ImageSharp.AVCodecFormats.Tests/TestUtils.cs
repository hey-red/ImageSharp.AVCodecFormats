using System.IO;
using System.Linq;
using System.Reflection;

using HeyRed.ImageSharp.AVCodecFormats.Avi;
using HeyRed.ImageSharp.AVCodecFormats.Mkv;
using HeyRed.ImageSharp.AVCodecFormats.Mov;
using HeyRed.ImageSharp.AVCodecFormats.Mp3;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using HeyRed.ImageSharp.AVCodecFormats.MpegTs;
using HeyRed.ImageSharp.AVCodecFormats.Webm;
using HeyRed.ImageSharp.AVCodecFormats.Wmv;

using SixLabors.ImageSharp;

namespace ImageSharp.AVCodecFormats.Tests
{
    public static class TestUtils
    {
        private const string TEST_DIR = "TestData";

        public static string GetTestDataPath()
        {
            var assemblyLocation = typeof(TestUtils).GetTypeInfo().Assembly.Location;
            var assemblyFile = new FileInfo(assemblyLocation);
            var directoryInfo = assemblyFile.Directory;

            while (!directoryInfo.EnumerateDirectories(TEST_DIR).Any())
            {
                directoryInfo = directoryInfo.Parent;
            }

            return Path.Combine(directoryInfo.FullName, TEST_DIR);
        }

        public static Configuration GetImageSharpConfiguration() => new Configuration(
            new AviConfigurationModule(),
            new MkvConfigurationModule(),
            new MovConfigurationModule(),
            new Mp4ConfigurationModule(),
            new WebmConfigurationModule(),
            new WmvConfigurationModule(),
            new MpegTsConfigurationModule(),
            new Mp3ConfigurationModule());
    }
}