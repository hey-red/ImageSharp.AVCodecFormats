using System;
using System.IO;
using System.Threading;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;

namespace HeyRed.ImageSharp.AVCodecFormats.Mkv;

public sealed class MkvDecoder : SpecializedImageDecoder<AVDecoderOptions>
{
    private MkvDecoder()
    {
    }

    /// <summary>
    /// Gets the shared instance.
    /// </summary>
    public static MkvDecoder Instance { get; } = new();

    /// <inheritdoc/>
    protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        return 
            new AVDecoderCore(new() { GeneralOptions = options })
            .Identify(stream, MkvFormat.Instance, cancellationToken);
    }

    /// <inheritdoc/>
    protected override Image<TPixel> Decode<TPixel>(AVDecoderOptions options, Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        Image<TPixel> image = 
            new AVDecoderCore(options)
            .Decode<TPixel>(stream, MkvFormat.Instance, cancellationToken);

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
