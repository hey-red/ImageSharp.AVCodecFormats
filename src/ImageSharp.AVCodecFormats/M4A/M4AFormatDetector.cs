using System;
using System.Diagnostics.CodeAnalysis;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.M4A;

public sealed class M4AFormatDetector : IImageFormatDetector
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
            header[4..8].SequenceEqual(M4AConstants.FtypMarker))
        {
            return
                header[8..12].SequenceEqual(M4AConstants.M4ABrandHeader);
        }

        return false;
    }
}