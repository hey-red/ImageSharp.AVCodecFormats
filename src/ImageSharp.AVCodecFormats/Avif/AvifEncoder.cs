using System;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats.Avif;

using SixLabors.ImageSharp.Formats;

public sealed class AvifEncoder : ImageEncoder
{
    protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}