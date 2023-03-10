using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Webm;

public sealed class WebmFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 39;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? WebmFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return
                header[..4].SequenceEqual(WebmConstants.WebmContainerStart) &&
                header[4..].IndexOf(WebmConstants.WebmMarker) > -1;
        }

        return false;
    }
}