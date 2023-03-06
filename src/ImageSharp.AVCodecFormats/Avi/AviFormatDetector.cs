using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avi;

public sealed class AviFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 16;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? AviFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return
                header[..4].SequenceEqual(AviConstants.RiffContainerHeader) &&
                header[8..16].SequenceEqual(AviConstants.AviListHeader);
        }

        return false;
    }
}