using HeyRed.ImageSharp.AVCodecFormats.Avi;
using HeyRed.ImageSharp.AVCodecFormats.Mkv;
using HeyRed.ImageSharp.AVCodecFormats.Mov;
using HeyRed.ImageSharp.AVCodecFormats.Mp3;
using HeyRed.ImageSharp.AVCodecFormats.Mp4;
using HeyRed.ImageSharp.AVCodecFormats.MpegTs;
using HeyRed.ImageSharp.AVCodecFormats.Webm;
using HeyRed.ImageSharp.AVCodecFormats.Wmv;

using SixLabors.ImageSharp.Metadata;

namespace HeyRed.ImageSharp.AVCodecFormats;

/// <summary>
/// Extension methods for the <see cref="ImageMetadata"/> type.
/// </summary>
public static partial class MetadataExtensions
{
    /// <summary>
    /// Gets the avi format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetAviMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(AviFormat.Instance);

    /// <summary>
    /// Gets the mkv format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetMkvMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(MkvFormat.Instance);

    /// <summary>
    /// Gets the mov format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetMovMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(MovFormat.Instance);

    /// <summary>
    /// Gets the mp3 format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetMp3Metadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(Mp3Format.Instance);

    /// <summary>
    /// Gets the mp4 format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetMp4Metadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(Mp4Format.Instance);

    /// <summary>
    /// Gets the mpeg ts format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetMpegTsMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(MpegTsFormat.Instance);

    /// <summary>
    /// Gets the webm format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetWebmMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(WebmFormat.Instance);

    /// <summary>
    /// Gets the wmv format specific metadata for the file.
    /// </summary>
    /// <param name="metadata">The metadata this method extends.</param>
    /// <returns>The <see cref="AVMetadata"/>.</returns>
    public static AVMetadata GetWmvMetadata(this ImageMetadata metadata) => metadata.GetFormatMetadata(WmvFormat.Instance);
}
