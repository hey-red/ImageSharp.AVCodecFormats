using System;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.M4A;

public sealed class M4AFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 12;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? M4AFormat.Instance : null;

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