using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mov;

/// <summary>
/// TODO: mdat, wide etc.
/// </summary>
public sealed class MovFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 10;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? MovFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return header[4..10].SequenceEqual(MovConstants.FtypQtMarker);
        }

        return false;
    }
}