using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFmpeg.AutoGen;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats;

public unsafe class AVMetadata : IFormatMetadata<AVMetadata>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AVMetadata" /> class.
    /// </summary>
    public AVMetadata()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AVMetadata" /> class.
    /// </summary>
    /// <param name="other">The metadata to create an instance from.</param>
    private AVMetadata(AVMetadata other)
    {
        ContainerFormat = other.ContainerFormat;
        Bitrate = other.Bitrate;
        Duration = other.Duration;
        ContainerMetadata = new(other.ContainerMetadata);
        VideoStreams = [.. other.VideoStreams];
        AudioStreams = [.. other.AudioStreams];
        PixelFormat = other.PixelFormat;
    }

    /// <summary>
    ///     Gets the container format name.
    /// </summary>
    public string ContainerFormat { get; internal set; } = string.Empty;

    /// <summary>
    ///     Gets the container bitrate in bytes per second (B/s) units. 0 if unknown.
    /// </summary>
    public long Bitrate { get; internal set; }

    /// <summary>
    ///     Gets the duration of the media container.
    /// </summary>
    public TimeSpan Duration { get; internal set; }

    /// <summary>
    ///     Gets the container file metadata. Streams may contain additional metadata.
    /// </summary>
    public Dictionary<string, string> ContainerMetadata { get; set; } = new();

    public IReadOnlyCollection<VideoStreamInfo> VideoStreams { get; set; } = Array.Empty<VideoStreamInfo>();

    public IReadOnlyCollection<AudioStreamInfo> AudioStreams { get; set; } = Array.Empty<AudioStreamInfo>();

    public string PixelFormat { get; set; } = string.Empty;

    public static AVMetadata FromFormatConnectingMetadata(FormatConnectingMetadata metadata) => new();
    
    public PixelTypeInfo GetPixelTypeInfo()
    {
        var bitsPerPixel = 0;
        AVPixelFormat pixFormat = ffmpeg.av_get_pix_fmt(PixelFormat);
        var desc = ffmpeg.av_pix_fmt_desc_get(pixFormat);
        if (desc != null)
        {
            bitsPerPixel = ffmpeg.av_get_bits_per_pixel(desc);
        }

        return new PixelTypeInfo(bitsPerPixel);
    }

    public FormatConnectingMetadata ToFormatConnectingMetadata()
    {
        return new FormatConnectingMetadata {
            AnimateRootFrame = true,
            BackgroundColor = Color.Transparent,
            PixelTypeInfo = this.GetPixelTypeInfo(),
        };
    }

    public void AfterImageApply<TPixel>(Image<TPixel> destination, Matrix4x4 matrix) where TPixel : unmanaged, IPixel<TPixel>
    {
    }

    /// <inheritdoc/>
    IDeepCloneable IDeepCloneable.DeepClone() => this.DeepClone();

    /// <inheritdoc/>
    public AVMetadata DeepClone() => new(this);
 
}