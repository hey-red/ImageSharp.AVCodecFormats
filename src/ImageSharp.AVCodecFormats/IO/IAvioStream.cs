namespace HeyRed.ImageSharp.AVCodecFormats.IO
{
    public unsafe interface IAvioStream
    {
        int ReadBufferLength { get; }

        int Read(void* opaque, byte* buffer, int bufferLength);

        long Seek(void* opaque, long offset, int whence);
    }
}