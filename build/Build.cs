using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Nuke.Common;

using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

namespace Build
{
    public class Build : NukeBuild
    {
        private const string NATIVE_VERSION = "1.3.0";

        private readonly IEnumerable<string> _architectures = new[] { "linux-x64", "win-x64" };

        private readonly string _packagesPath = Path.Combine(RootDirectory, "packages");

        private readonly string _binPath = Path.Combine(RootDirectory, "build", "binaries");

        private Target DownloadBinaries => _ => _
             .Executes(async () =>
             {
                 var httpClient = new HttpClient();

                 foreach (var architecture in _architectures)
                 {
                     string libTmpPath = Path.Combine(_binPath, architecture);
                     string tmpFile = Path.GetTempFileName();

                     var uri = $"https://github.com/hey-red/FFmpeg-Build/releases/download/{NATIVE_VERSION}/{architecture}-noencoders.tar.gz";

                     Logger.Info($"Download tarball from {uri}");
                     using var response = await httpClient.GetStreamAsync(uri);

                     using (var fs = File.OpenWrite(tmpFile))
                     {
                         await response.CopyToAsync(fs);
                     }

                     Logger.Info($"Extract tarball to {libTmpPath}");
                     BuildUtils.ExtractArchive(tmpFile, libTmpPath);

                     File.Delete(tmpFile);
                 }
             });

        private Target CreateNuGetPackages => _ => _
             .DependsOn(DownloadBinaries)
             .Executes(() =>
             {
                 foreach (var architecture in _architectures)
                 {
                     NuGetPack(p => p
                         .SetTargetPath(RootDirectory / "build/ImageSharp.AVCodecFormats.Native." + architecture + ".nuspec")
                         .SetVersion(NATIVE_VERSION)
                         .SetOutputDirectory(_packagesPath)
                         .AddProperty("NoWarn", "NU5128"));
                 }

                 NuGetPack(p => p
                     .SetTargetPath(RootDirectory / "build/ImageSharp.AVCodecFormats.Native.nuspec")
                     .SetVersion(NATIVE_VERSION)
                     .SetOutputDirectory(_packagesPath)
                     .AddProperty("NoWarn", "NU5128"));
             });

        private Target CleanUp => _ => _
             .Executes(() =>
             {
                 EnsureCleanDirectory(_packagesPath);
                 EnsureCleanDirectory(_binPath);
             });

        private Target All => _ => _
                    .DependsOn(CleanUp)
                    .DependsOn(CreateNuGetPackages);

        public static int Main() => Execute<Build>(x => x.All);
    }
}