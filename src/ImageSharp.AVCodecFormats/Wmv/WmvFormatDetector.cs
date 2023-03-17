using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv;

public sealed class WmvFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 16;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? WmvFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return header[..16].SequenceEqual(WmvConstants.AsfFormatHeader);
        }

        return false;
    }
}