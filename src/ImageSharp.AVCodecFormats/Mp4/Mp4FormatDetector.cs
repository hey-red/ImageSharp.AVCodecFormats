using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

public sealed class Mp4FormatDetector : IImageFormatDetector
{
    public int HeaderSize => 12;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? Mp4Format.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize &&
            header[4..8].SequenceEqual(Mp4Constants.FtypMarker))
        {
            // TODO: move to type markers
            var subHeader = header[8..];
            return
                // MP4 Base Media v1 [IS0 14496-12:2003]
                (
                    subHeader[0] == 0x69 &&     // i
                    subHeader[1] == 0x73 &&     // s
                    subHeader[2] == 0x6F &&     // o
                    subHeader[3] == 0x6D        // m
                )
                ||
                // MP4 Base Media v2 [ISO 14496-12:2005]
                (
                    subHeader[0] == 0x69 &&     // i
                    subHeader[1] == 0x73 &&     // s
                    subHeader[2] == 0x6F &&     // o
                    subHeader[3] == 0x32        // 2
                )
                ||
                // 4
                (
                    subHeader[0] == 0x69 &&     // i
                    subHeader[1] == 0x73 &&     // s
                    subHeader[2] == 0x6F &&     // o
                    subHeader[3] == 0x34        // 4
                )
                ||
                // 5
                (
                    subHeader[0] == 0x69 &&     // i
                    subHeader[1] == 0x73 &&     // s
                    subHeader[2] == 0x6F &&     // o
                    subHeader[3] == 0x35        // 5
                )
                ||
                // 6
                (
                    subHeader[0] == 0x69 &&     // i
                    subHeader[1] == 0x73 &&     // s
                    subHeader[2] == 0x6F &&     // o
                    subHeader[3] == 0x36        // 6
                )
                ||
                // MP4 Base w/ AVC ext [ISO 14496-12:2005]
                (
                    subHeader[0] == 0x61 &&     // a
                    subHeader[1] == 0x76 &&     // v
                    subHeader[2] == 0x63 &&     // c
                    subHeader[3] == 0x31        // 1
                )
                ||
                // MP4 v1 [ISO 14496-1:ch13]
                (
                    subHeader[0] == 0x6D &&     // m
                    subHeader[1] == 0x70 &&     // p
                    subHeader[2] == 0x34 &&     // 4
                    subHeader[3] == 0x31        // 1
                )
                ||
                // MP4 v2 [ISO 14496-14]
                (
                    subHeader[0] == 0x6D &&     // m
                    subHeader[1] == 0x70 &&     // p
                    subHeader[2] == 0x34 &&     // 4
                    subHeader[3] == 0x32        // 2
                )
                ||
                // MPEG v4 system
                // Dynamic Adaptive Streaming over HTTP
                (
                    subHeader[0] == 0x64 &&     // d
                    subHeader[1] == 0x61 &&     // a
                    subHeader[2] == 0x73 &&     // s
                    subHeader[3] == 0x68        // h
                )
                ||
                // Apple M4V
                (
                    subHeader[0] == 0x4D &&     // M
                    subHeader[1] == 0x34 &&     // 4
                    subHeader[2] == 0x56        // V
                );
        }

        return false;
    }
}