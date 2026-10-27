using System;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

public sealed class AvifDecoder : SpecializedImageDecoder<AVDecoderOptions>
{
    private AvifDecoder()
    {
    }

    /// <summary>
    ///     Gets the shared instance.
    /// </summary>
    public static AvifDecoder Instance { get; } = new AvifDecoder();

    /// <inheritdoc />
    protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(stream);

        return
            new AVDecoderCore(new() { GeneralOptions = options })
                .Identify(stream, AvifFormat.Instance, cancellationToken);
    }

    /// <inheritdoc />
    protected override Image<TPixel> Decode<TPixel>(
        AVDecoderOptions options,
        Stream stream,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(stream);

        var image =
            new AVDecoderCore(options)
                .Decode<TPixel>(stream, AvifFormat.Instance, cancellationToken);

        ScaleToTargetSize(options.GeneralOptions, image);

        return image;
    }

    /// <inheritdoc />
    protected override Image Decode(AVDecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        return Decode<Rgba32>(options, stream, cancellationToken);
    }

    /// <inheritdoc />
    protected override AVDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
    {
        return new() { GeneralOptions = options };
    }
}