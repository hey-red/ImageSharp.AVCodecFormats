using System;

using SixLabors.ImageSharp;

namespace HeyRed.ImageSharp.AVCodecFormats;

internal static class ResizeHelper
{
    public static Size CalculateMaxRectangle(
        Size source,
        int width,
        int height)
    {
        var targetWidth = width;
        var targetHeight = height;

        // Fractional variants for preserving aspect ratio.
        var percentHeight = MathF.Abs(height / (float)source.Height);
        var percentWidth = MathF.Abs(width / (float)source.Width);

        // Integers must be cast to floats to get needed precision
        var ratio = height / (float)width;
        var sourceRatio = source.Height / (float)source.Width;

        if (sourceRatio < ratio)
        {
            targetHeight = (int)MathF.Round(source.Height * percentWidth);
        }
        else
        {
            targetWidth = (int)MathF.Round(source.Width * percentHeight);
        }

        return new Size(Sanitize(targetWidth), Sanitize(targetHeight));
    }

    private static int Sanitize(int input) => Math.Max(1, input);
}