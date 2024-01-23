using System;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats;

public sealed class VideoStreamInfo
{
    /// <summary>
    /// Gets the codec name.
    /// </summary>
    public string CodecName { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the stream duration.
    /// </summary>
    public TimeSpan Duration { get; internal set; }

    /// <summary>
    /// Gets the average frame rate as a <see cref="double"/> value.
    /// </summary>
    public double AvgFrameRate { get; internal set; }

    /// <summary>
    /// Gets the number of frames value taken from the container metadata or estimated in constant frame rate videos. 
    /// Returns <see langword="null"/> if not available.
    /// </summary>
    public int? FramesCount { get; internal set; }

    /// <summary>
    /// Gets the clockwise rotation angle computed from the display matrix.
    /// </summary>
    public double Rotation { get; internal set; }

    /// <summary>
    /// Gets sample aspect ratio.
    /// 0 if unknown.
    /// </summary>
    public AVRational SampleAspectRatio { get; internal set; }
}
