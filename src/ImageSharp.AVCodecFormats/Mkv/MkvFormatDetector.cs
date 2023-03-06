using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv;

public sealed class MkvFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 39;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? MkvFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return
                header[..4].SequenceEqual(MkvConstants.MkvContainerStart) &&
                header[4..].IndexOf(MkvConstants.MatroskaMarker) > -1;
        }

        return false;
    }
}