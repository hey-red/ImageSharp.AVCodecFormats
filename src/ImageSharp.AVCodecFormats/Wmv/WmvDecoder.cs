using System;
using System.IO;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats.Wmv;

public sealed class WmvDecoder : SpecializedImageDecoder<AVDecoderOptions>
{
    private WmvDecoder()
    {
    }

    /// <summary>
    ///     Gets the shared instance.
    /// </summary>
    public static WmvDecoder Instance { get; } = new();

    /// <inheritdoc />
    protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        return
            new AVDecoderCore(new AVDecoderOptions { GeneralOptions = options })
                .Identify(stream, WmvFormat.Instance, cancellationToken);
    }

    /// <inheritdoc />
    protected override Image<TPixel> Decode<TPixel>(
        AVDecoderOptions options,
        Stream stream,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        var image =
            new AVDecoderCore(options)
                .Decode<TPixel>(stream, WmvFormat.Instance, cancellationToken);

        ScaleToTargetSize(options.GeneralOptions, image);

        return image;
    }

    /// <inheritdoc />
    protected override Image Decode(AVDecoderOptions options, Stream stream, CancellationToken cancellationToken)
        => Decode<Rgba32>(options, stream, cancellationToken);

    /// <inheritdoc />
    protected override AVDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
        => new() { GeneralOptions = options };
}