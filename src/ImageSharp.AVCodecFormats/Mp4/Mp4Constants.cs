using System.Collections.Generic;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

internal static class Mp4Constants
{
    public static readonly IEnumerable<string> FileExtensions = new[] { "mp4" };

    public static readonly IEnumerable<string> MimeTypes = new[] { "video/mp4", "application/mp4" };

    /// <summary>
    /// ISO Media
    /// </summary>
    public static readonly byte[] FtypMarker =
    {
        0x66,   // f
        0x74,   // t
        0x79,   // y
        0x70,   // p
    };

    /// <summary>
    /// MP4 Base Media v1 [IS0 14496-12:2003]
    /// </summary>
    public static readonly byte[] IsomBrandHeader =
    {
        0x69,   // i
        0x73,   // s
        0x6F,   // o
        0x6D,   // m
    };

    /// <summary>
    /// v2
    /// </summary>
    public static readonly byte[] Iso2BrandHeader =
    {
        0x69,   // i
        0x73,   // s
        0x6F,   // o
        0x32,   // 2
    };

    /// <summary>
    /// v4
    /// </summary>
    public static readonly byte[] Iso4BrandHeader =
    {
        0x69,   // i
        0x73,   // s
        0x6F,   // o
        0x34,   // 4
    };

    /// <summary>
    /// v5
    /// </summary>
    public static readonly byte[] Iso5BrandHeader =
    {
        0x69,   // i
        0x73,   // s
        0x6F,   // o
        0x35,   // 5
    };

    /// <summary>
    /// v6
    /// </summary>
    public static readonly byte[] Iso6BrandHeader =
    {
        0x69,   // i
        0x73,   // s
        0x6F,   // o
        0x36,   // 6
    };

    /// <summary>
    /// MP4 Base w/ AVC ext [ISO 14496-12:2005]
    /// </summary>
    public static readonly byte[] Avc1BrandHeader =
    {
        0x61,   // a
        0x76,   // v
        0x63,   // c
        0x31,   // 1
    };

    /// <summary>
    /// MPEG-21 [ISO/IEC 21000-9]
    /// </summary>
    public static readonly byte[] Mp21BrandHeader =
    {
        0x6D,   // m
        0x70,   // p
        0x32,   // 2
        0x31,   // 1
    };

    /// <summary>
    /// MP4 v1 [ISO 14496-1:ch13]
    /// </summary>
    public static readonly byte[] Mp41BrandHeader =
    {
        0x6D,   // m
        0x70,   // p
        0x34,   // 4
        0x31,   // 1
    };

    /// <summary>
    /// MP4 v2 [ISO 14496-14]
    /// </summary>
    public static readonly byte[] Mp42BrandHeader =
    {
        0x6D,   // m
        0x70,   // p
        0x34,   // 4
        0x32,   // 2
    };

    /// <summary>
    /// MPEG v4 system, 
    /// Dynamic Adaptive Streaming over HTTP
    /// </summary>
    public static readonly byte[] DashBrandHeader =
    {
        0x64,   // d
        0x61,   // a
        0x73,   // s
        0x68,   // h
    };

    /// <summary>
    /// Apple iTunes Video (.M4V) Video
    /// </summary>
    public static readonly byte[] M4vBrandHeader =
    {
        0x4D,   // M
        0x34,   // 4
        0x56,   // V
        0x20,
    };
}
