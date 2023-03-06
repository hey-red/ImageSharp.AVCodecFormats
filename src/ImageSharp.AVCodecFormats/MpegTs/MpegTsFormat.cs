using System.Collections.Generic;

using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats.MpegTs;

public sealed class MpegTsFormat : IImageFormat<MpegTsMetadata>
{
    private MpegTsFormat()
    {
    }

    public static MpegTsFormat Instance { get; } = new();

    public string Name => "MPEG Transport Stream";

    public string DefaultMimeType => "video/MP2T";

    public IEnumerable<string> MimeTypes => MpegTsConstants.MimeTypes;

    public IEnumerable<string> FileExtensions => MpegTsConstants.FileExtensions;

    public MpegTsMetadata CreateDefaultFormatMetadata() => new();
}