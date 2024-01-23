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

    /// <summary>
    /// Gets or sets a value indicating whether frames should be scaled based on SAR value(if set).
    /// </summary>
    public bool RespectSampleAspectRatio { get; set; }

    /// <summary>
    /// A delegate that provides the way to skip frames based on their content.
    /// The first argument is <see cref="ImageFrame"/>, the second is the current frame number.
    /// Returns true, when frame should be skipped, otherwise false.
    /// </summary>
    public Func<ImageFrame, uint, bool>? FrameFilter { get; set; }
}