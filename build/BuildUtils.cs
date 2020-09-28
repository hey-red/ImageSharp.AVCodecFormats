using System.IO;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Build
{
    public static class BuildUtils
    {
        public static void ExtractArchive(string tarballFileName, string destDirectory)
        {
            var inputStream = File.OpenRead(tarballFileName);
            var gzipStream = new GZipInputStream(inputStream);

            var tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents(destDirectory);
            tarArchive.Close();

            inputStream.Close();
            gzipStream.Close();
        }
    }
}
