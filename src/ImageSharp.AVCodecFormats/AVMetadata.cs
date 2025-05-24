using System;
using System.Collections.Generic;

using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats;

public class AVMetadata : IDeepCloneable
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
        ContainerMetadata = other.ContainerMetadata;
        VideoStreams = other.VideoStreams;
        AudioStreams = other.AudioStreams;
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
    public Dictionary<string, string> ContainerMetadata { get; internal set; } = new();

    public IReadOnlyCollection<VideoStreamInfo> VideoStreams { get; internal set; } = Array.Empty<VideoStreamInfo>();

    public IReadOnlyCollection<AudioStreamInfo> AudioStreams { get; internal set; } = Array.Empty<AudioStreamInfo>();

    public IDeepCloneable DeepClone() => new AVMetadata(this);
}