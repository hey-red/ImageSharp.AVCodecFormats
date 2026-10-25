using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using FFMediaToolkit;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats;

internal static class FFmpegBinariesFinder
{
    internal static void FindBinaries()
    {
        if (string.IsNullOrWhiteSpace(FFmpegBinaries.Path))
        {
            var libPath = FindFFmpegLibraryPath();
            if (libPath != null)
            {
                FFmpegLoader.FFmpegPath = libPath; // override default path(ffmpeg.RootPath)
                FFmpegBinaries.Path = libPath;
            }
        }
    }

    private static PlatformID GetCurrentPlatformId()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return PlatformID.Win32NT;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return PlatformID.Unix;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return PlatformID.MacOSX;
        }

        throw new PlatformNotSupportedException();
    }

    private static string? FindFFmpegLibraryPath()
    {
        PlatformID currentPlatform = GetCurrentPlatformId();
        
        var currentPath = AppContext.BaseDirectory;

        // Deconstruct available since netstandart2.1 :(
        var probeLib = ffmpeg.LibraryVersionMap.First();
        var libName = probeLib.Key;
        var libVersion = probeLib.Value;

        var (rid, libNameWithVersion) = currentPlatform switch
        {
            PlatformID.Win32NT => ("win", $"{libName}-{libVersion}.dll"),
            PlatformID.Unix => ("linux", $"lib{libName}.so.{libVersion}"),
            _ => ("osx", $"lib{libName}.{libVersion}.dylib")
        };

        // Find inside current path
        var libPath = Path.Combine(currentPath, libNameWithVersion);
        if (File.Exists(libPath))
        {
            return currentPath;
        }

        var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        var runtimePath = Path.Combine(currentPath, $"runtimes/{rid}-{architecture}/native/");

        // Find inside runtime directory
        libPath = Path.Combine(runtimePath, libNameWithVersion);
        if (File.Exists(libPath))
        {
            return runtimePath;
        }

        // Find inside system dirs(linux only)
        if (currentPlatform == PlatformID.Unix)
        {
            foreach (var systemPath in GetLinuxSystemLibraryPaths())
            {
                if (File.Exists(Path.Combine(systemPath, libNameWithVersion)))
                {
                    return systemPath;
                }
            }
        }

        // TODO: LD_LIBRARY_PATH, PATH on windows

        return null;
    }
    
    private static IEnumerable<string> GetLinuxSystemLibraryPaths()
    {
        var triplet = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x86_64-linux-gnu",
            Architecture.Arm64 => "aarch64-linux-gnu",
            Architecture.Arm => "arm-linux-gnueabihf",
            Architecture.X86 => "i386-linux-gnu",
            _ => null
        };

        if (triplet != null)
        {
            yield return $"/usr/lib/{triplet}";
        }

        yield return "/usr/lib64";
        yield return "/usr/lib";
        yield return "/usr/local/lib";
    }
}