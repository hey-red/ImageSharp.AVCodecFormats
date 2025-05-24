using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp3;

public sealed class Mp3FormatDetector : IImageFormatDetector
{
    public int HeaderSize => 51200;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? Mp3Format.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return
                header[..3].SequenceEqual(Mp3Constants.Id3Marker) &&
                // Match PIC or APIC
                header[3..].IndexOf(Mp3Constants.PicMarker) > -1;
        }

        return false;
    }
}