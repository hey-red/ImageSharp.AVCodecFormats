using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tools.DotNet;

using Serilog;

using static Fallout.Common.Tools.DotNet.DotNetTasks;

namespace Build;

public class Build : FalloutBuild
{
    private const string NativeVersion = "4.0.0";

    private static readonly string[] Architectures =
    {
        "linux-x64",
        "win-x64"
    };

    private readonly AbsolutePath _binPath = RootDirectory / "build" / "binaries";
    private readonly AbsolutePath _packagesPath = RootDirectory / "packages";
    private readonly AbsolutePath _localFeedPath = RootDirectory / "artifacts" / "nuget-feed";

    private void PackRuntimeProject(string projectName)
    {
        DotNetPack(p => p
            .SetProject(RootDirectory / "packaging" / projectName)
            .SetVersion(NativeVersion)
            .SetOutputDirectory(_localFeedPath)
            .SetProperty("NoWarn", "NU5128"));
    }

    private void PackMetaProject()
    {
        DotNetPack(p => p
            .SetProject(RootDirectory / "packaging" / "ImageSharp.AVCodecFormats.Native.csproj")
            .SetVersion(NativeVersion)
            .SetOutputDirectory(_packagesPath)
            .SetProperty("NoWarn", "NU5128")
            .SetProperty("RestoreSources", _localFeedPath.ToString()));
    }

    private Target DownloadBinaries => _ => _
        .Executes(async () =>
        {
            using var httpClient = new HttpClient();

            foreach (var architecture in Architectures)
            {
                var libTmpPath = _binPath / architecture;
                var tmpFile = Path.GetTempFileName();

                var uri =
                    $"https://github.com/hey-red/FFmpeg-Build/releases/download/{NativeVersion}/{architecture}-noencoders.tar.gz";

                Log.Information($"Download tarball from {uri}");

                await using (var response = await httpClient.GetStreamAsync(uri))
                {
                    await using var fs = File.OpenWrite(tmpFile);
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
            _localFeedPath.CreateOrCleanDirectory();
            _packagesPath.CreateOrCleanDirectory();

            PackRuntimeProject("ImageSharp.AVCodecFormats.Native.linux-x64.csproj");
            PackRuntimeProject("ImageSharp.AVCodecFormats.Native.win-x64.csproj");

            PackMetaProject();
        });

    private Target CleanUp => _ => _
        .Executes(() =>
        {
            _packagesPath.CreateOrCleanDirectory();
            _localFeedPath.CreateOrCleanDirectory();
            _binPath.CreateOrCleanDirectory();
        });

    private Target All => _ => _
        .DependsOn(CleanUp)
        .DependsOn(CreateNuGetPackages);

    public static int Main() => Execute<Build>(x => x.All);
}