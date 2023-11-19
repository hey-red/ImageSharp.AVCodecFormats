using System;
using System.Diagnostics.CodeAnalysis;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.Ogg;

/// <summary>
/// TODO: detect theora/OGM
/// COVERART/METADATA_BLOCK_PICTURE for opus, vorbis and FLAC
/// index >>28
/// </summary>
public sealed class OggFormatDetector : IImageFormatDetector
{
    public int HeaderSize => 28;

    public bool TryDetectFormat(ReadOnlySpan<byte> header, [NotNullWhen(true)] out IImageFormat? format)
    {
        format = IsSupportedFileFormat(header) ? OggFormat.Instance : null;

        return format != null;
    }

    private bool IsSupportedFileFormat(ReadOnlySpan<byte> header)
    {
        if (header.Length >= HeaderSize)
        {
            return
                header[..4].SequenceEqual(OggConstants.OggMarker);
        }

        return false;
    }
}