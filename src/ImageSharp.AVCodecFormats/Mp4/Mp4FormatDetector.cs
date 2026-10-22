using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

public sealed class Mp4FormatDetector : IImageFormatDetector
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
            header[4..8].SequenceEqual(Mp4Constants.FtypMarker))
        {
            var subHeader = header[8..12];

            return
                subHeader.SequenceEqual(Mp4Constants.IsomBrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Iso2BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Iso4BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Iso5BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Iso6BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Avc1BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Mp21BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Mp41BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.Mp42BrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.DashBrandHeader) ||
                subHeader.SequenceEqual(Mp4Constants.M4vBrandHeader);
        }

        return false;
    }
}