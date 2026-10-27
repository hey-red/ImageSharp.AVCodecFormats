using System;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

public sealed class AvifImageFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 12;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? AvifFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
            return
                header.Slice(4, 4).SequenceEqual(AvifConstants.FtypHeader) &&
                ContainsSupportedBrand(header.Slice(8, 4));

        return false;
    }

    private static bool ContainsSupportedBrand(ReadOnlySpan<byte> header)
    {
        return
            header.SequenceEqual(AvifConstants.AvifBrandHeader) ||
            header.SequenceEqual(AvifConstants.AvisBrandHeader);
    }
}