using HeyRed.ImageSharp.AVCodecFormats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class AVMetadataCloneTests
{
    [Fact]
    public void DeepClone_DoesNotShareContainerMetadata()
    {
        var original = new AVMetadata();
        original.ContainerMetadata["title"] = "original";

        AVMetadata clone = original.DeepClone();
        clone.ContainerMetadata["title"] = "changed";
        clone.ContainerMetadata["extra"] = "value";

        Assert.Equal("original", original.ContainerMetadata["title"]);
        Assert.False(original.ContainerMetadata.ContainsKey("extra"));
    }

    [Fact]
    public void DeepClone_CopiesScalarValues()
    {
        var original = new AVMetadata { PixelFormat = "yuv420p" };

        AVMetadata clone = original.DeepClone();

        Assert.Equal("yuv420p", clone.PixelFormat);
        Assert.NotSame(original, clone);
    }
}