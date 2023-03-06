using System.IO;
using System.Linq;
using System.Reflection;

namespace ImageSharp.AVCodecFormats.Tests;

public static class TestHelpers
{
    private const string TEST_DIR = "TestData";

    public static string GetTestDataPath()
    {
        var assemblyLocation = typeof(TestHelpers).GetTypeInfo().Assembly.Location;
        var assemblyFile = new FileInfo(assemblyLocation);
        var directoryInfo = assemblyFile.Directory;

        while (!directoryInfo.EnumerateDirectories(TEST_DIR).Any())
        {
            directoryInfo = directoryInfo.Parent;
        }

        return Path.Combine(directoryInfo.FullName, TEST_DIR);
    }
}