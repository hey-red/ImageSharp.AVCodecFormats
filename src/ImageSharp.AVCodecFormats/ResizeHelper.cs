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
        int targetWidth = width;
        int targetHeight = height;

        // Fractional variants for preserving aspect ratio.
        float percentHeight = MathF.Abs(height / (float)source.Height);
        float percentWidth = MathF.Abs(width / (float)source.Width);

        // Integers must be cast to floats to get needed precision
        float ratio = height / (float)width;
        float sourceRatio = source.Height / (float)source.Width;

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
