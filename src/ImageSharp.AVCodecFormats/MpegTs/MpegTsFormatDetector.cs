using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs;

public sealed class MpegTsFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 189;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? MpegTsFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            // Every 188
            return
                header[0] == MpegTsConstants.G_MARK &&
                header[188] == MpegTsConstants.G_MARK;
        }

        return false;
    }
}