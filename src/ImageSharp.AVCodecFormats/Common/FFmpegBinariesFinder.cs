using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using FFMediaToolkit;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats.Common
{
    internal static class FFmpegBinariesFinder
    {
        internal static void FindBinaries()
        {
            if (string.IsNullOrWhiteSpace(FFmpegBinaries.Path))
            {
                string? libPath = FindFFmpegLibraryPath();
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
                return PlatformID.Win32NT;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return PlatformID.Unix;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return PlatformID.MacOSX;
            throw new PlatformNotSupportedException();
        }

        private static string? FindFFmpegLibraryPath()
        {
            PlatformID currentPlatform = GetCurrentPlatformId();
            // Should be Environment.CurrentDirectory, but
            // https://github.com/dotnet/project-system/issues/5053
            var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Deconstruct available since netstandart2.1 :(
            var probeLib = ffmpeg.LibraryVersionMap.First();
            var libName = probeLib.Key;
            int libVersion = probeLib.Value;

            var (rid, libNameWithVersion) = currentPlatform switch
            {
                PlatformID.Win32NT => ("win", $"{libName}-{libVersion}.dll"),
                PlatformID.Unix => ("linux", $"lib{libName}.so.{libVersion}"),
                _ => ("osx", $"lib{libName}.{libVersion}.dylib"),
            };

            // Find inside current path
            string libPath = Path.Combine(currentPath, libNameWithVersion);
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

            // Find inside system dir(linux only)
            if (currentPlatform == PlatformID.Unix)
            {
                var unixLibPath = $"/usr/lib/x86{(Environment.Is64BitOperatingSystem ? "_64" : "")}-linux-gnu";

                libPath = Path.Combine(unixLibPath, libNameWithVersion);
                if (File.Exists(libPath))
                {
                    return unixLibPath;
                }
            }

            // TODO: LD_LIBRARY_PATH, PATH on windows

            return null;
        }
    }
}