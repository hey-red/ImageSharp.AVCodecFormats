using System;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4
{
    public sealed class Mp4FormatDetector : IImageFormatDetector
    {
        public int HeaderSize => 8;

        public IImageFormat? DetectFormat(ReadOnlySpan<byte> header)
        {
            return IsSupportedFileFormat(header) ? Mp4Format.Instance : null;
        }

        private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
        {
            if (header.Length >= HeaderSize &&
                // Base header
                header[4] == 0x66 &&    // f
                header[5] == 0x74 &&    // t
                header[6] == 0x79 &&    // y
                header[7] == 0x70)      // p
            {
                var subHeader = header.Slice(8);
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
                    );
            }

            return false;
        }
    }
}