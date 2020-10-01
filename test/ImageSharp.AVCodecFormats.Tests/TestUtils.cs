using System.IO;
using System.Linq;
using System.Reflection;

using HeyRed.ImageSharp.AVCodecFormats;

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

        public static Configuration GetImageSharpConfiguration() => new Configuration().WithAVDecoders();
    }
}