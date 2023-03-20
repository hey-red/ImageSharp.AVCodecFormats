using System;
using System.IO;
using System.Linq;

using HeyRed.ImageSharp.AVCodecFormats;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

using Xunit;

namespace ImageSharp.AVCodecFormats.Tests;

public class MetadataTests
{
    private readonly string _testVideoDataPath;

    private readonly DecoderOptions _decoderOptions;

    public MetadataTests()
    {
        _testVideoDataPath = TestHelpers.GetTestDataPath();
        _decoderOptions = new DecoderOptions
        {
            MaxFrames = 1,
            Configuration = new Configuration().WithAVDecoders()
        };
    }

    [Fact]
    public void VideoWithTitleMetadataTest()
    {
        string filePath = Path.Combine(_testVideoDataPath, "vp9.webm");

        using var inputStream = File.OpenRead(filePath);
        AVMetadata metadata = Image.Identify(_decoderOptions, inputStream).Metadata.GetWebmMetadata();

        Assert.Equal("matroska,webm", metadata.ContainerFormat);
        Assert.Equal(1187707, metadata.Bitrate);
        Assert.Equal("Macross “September“ Delta", metadata.ContainerMetadata["title"]);

        var videoStream = metadata.VideoStreams.First();

        Assert.Equal("vp9", videoStream.CodecName);
        Assert.NotEqual(TimeSpan.MinValue, videoStream.Duration);
        Assert.Equal(30, videoStream.AvgFrameRate);
        Assert.Equal(330, videoStream.FramesCount);

        var audioStream = metadata.AudioStreams.First();

        Assert.Equal("opus", audioStream.CodecName);
        Assert.Equal(2, audioStream.NumChannels);
        Assert.NotEqual(TimeSpan.MinValue, audioStream.Duration);
    }

    [Fact]
    public void MpgaMetadataTest()
    {
        string filePath = Path.Combine(_testVideoDataPath, "mpga.mp3");

        using var inputStream = File.OpenRead(filePath);
        AVMetadata metadata = Image.Identify(_decoderOptions, inputStream).Metadata.GetMp3Metadata();

        Assert.Equal(323591, metadata.Bitrate);

        var videoStream = metadata.VideoStreams.First();
        Assert.Equal("mjpeg", videoStream.CodecName);
        Assert.NotEqual(TimeSpan.MinValue, videoStream.Duration);

        var audioStream = metadata.AudioStreams.First();
        Assert.Equal("mp3", audioStream.CodecName);
        Assert.NotEqual(TimeSpan.MinValue, audioStream.Duration);

        Assert.Equal("Kai Engel", metadata.ContainerMetadata["artist"]);
        Assert.Equal("Satin", metadata.ContainerMetadata["album"]);
        Assert.Equal("Augmentations", metadata.ContainerMetadata["title"]);
        Assert.Equal("2019-08-17T09:52:22", metadata.ContainerMetadata["date"]);
    }
}
