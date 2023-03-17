using System;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace HeyRed.ImageSharp.AVCodecFormats;

/// <summary>
/// AV decoder options for generating an image out of a video/audio stream.
/// </summary>
public sealed class AVDecoderOptions : ISpecializedDecoderOptions
{
    /// <inheritdoc/>
    public DecoderOptions GeneralOptions { get; init; } = new();

    /// <summary>
    /// Preserve aspect ratio when <see cref="DecoderOptions.TargetSize"/> is set.
    /// </summary>
    public bool PreserveAspectRatio { get; set; }

    public Func<ImageFrame, uint, bool>? FrameFilter { get; set; }
}