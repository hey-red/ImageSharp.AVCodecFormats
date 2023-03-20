using System;

namespace HeyRed.ImageSharp.AVCodecFormats;

public sealed class AudioStreamInfo
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
    /// Gets the number of audio channels stored in the stream.
    /// </summary>
    public int NumChannels { get; internal set; }

    /// <summary>
    /// Gets the number of samples per second of the audio stream.
    /// </summary>
    public int SampleRate { get; internal set; }
}
