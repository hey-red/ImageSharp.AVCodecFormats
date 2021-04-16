using System.IO;
using System.Text;

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

            var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.UTF8);
            tarArchive.ExtractContents(destDirectory);
            tarArchive.Close();

            inputStream.Close();
            gzipStream.Close();
        }
    }
}