using System;
using System.IO;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mp4;

public sealed class Mp4Decoder : SpecializedImageDecoder<AVDecoderOptions>
{
    private Mp4Decoder()
    {
    }

    /// <summary>
    /// Gets the shared instance.
    /// </summary>
    public static Mp4Decoder Instance { get; } = new();

    /// <inheritdoc/>
    protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        return 
            new AVDecoderCore(new() { GeneralOptions = options })
            .Identify(stream, Mp4Format.Instance, cancellationToken);
    }

    /// <inheritdoc/>
    protected override Image<TPixel> Decode<TPixel>(AVDecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        Image<TPixel> image = 
            new AVDecoderCore(options)
            .Decode<TPixel>(stream, Mp4Format.Instance, cancellationToken);

        ScaleToTargetSize(options.GeneralOptions, image);

        return image;
    }

    /// <inheritdoc/>
    protected override Image Decode(AVDecoderOptions options, Stream stream, CancellationToken cancellationToken)
        => Decode<Rgba32>(options, stream, cancellationToken);

    /// <inheritdoc/>
    protected override AVDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
        => new() { GeneralOptions = options };
}
