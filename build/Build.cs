using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.NuGet;

using Serilog;

using static Nuke.Common.Tools.NuGet.NuGetTasks;

namespace Build;

public class Build : NukeBuild
{
    private const string NATIVE_VERSION = "3.0.0";

    private readonly IEnumerable<string> _architectures = new[] { "linux-x64", "win-x64" };

    private readonly AbsolutePath _binPath = RootDirectory / "build" / "binaries";

    private readonly AbsolutePath _packagesPath = RootDirectory / "packages";

    private Target DownloadBinaries => _ => _
        .Executes(async () =>
        {
            var httpClient = new HttpClient();

            foreach (var architecture in _architectures)
            {
                var libTmpPath = Path.Combine(_binPath, architecture);
                var tmpFile = Path.GetTempFileName();

                var uri =
                    $"https://github.com/hey-red/FFmpeg-Build/releases/download/{NATIVE_VERSION}/{architecture}-noencoders.tar.gz";

                Log.Information($"Download tarball from {uri}");
                using Stream response = await httpClient.GetStreamAsync(uri);

                using (FileStream fs = File.OpenWrite(tmpFile))
                {
                    await response.CopyToAsync(fs);
                }

                Log.Information($"Extract tarball to {libTmpPath}");
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
            _packagesPath.CreateOrCleanDirectory();
            _binPath.CreateOrCleanDirectory();
        });

    private Target All => _ => _
        .DependsOn(CleanUp)
        .DependsOn(CreateNuGetPackages);

    public static int Main() => Execute<Build>(x => x.All);
}