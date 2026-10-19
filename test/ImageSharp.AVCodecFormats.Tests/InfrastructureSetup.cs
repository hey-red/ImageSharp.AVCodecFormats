using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFMediaToolkit;
using HeyRed.ImageSharp.AVCodecFormats;

namespace ImageSharp.AVCodecFormats.Tests;

public static class InfrastructureSetup
{
    [ModuleInitializer]
    public static void InitializeFFmpeg()
    {
        var baseDir = AppContext.BaseDirectory;
        var rid = GetCurrentRuntimeIdentifier();
        var nativePath = Path.Combine(baseDir, "runtimes", rid, "native");

        SetFFmpegPath(Directory.Exists(nativePath) ? nativePath : baseDir);
    }

    private static void SetFFmpegPath(string path)
    {
        FFmpegBinaries.Path = path;
        FFmpegLoader.FFmpegPath = path;
    }
    
    private static string GetCurrentRuntimeIdentifier()
    {
        var os = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx" : "unknown";

        var architecture = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.Arm64 => "arm64",
            Architecture.X86 => "x86",
            Architecture.Arm => "arm",
            _ => "unknown"
        };

        return $"{os}-{architecture}";
    }
}