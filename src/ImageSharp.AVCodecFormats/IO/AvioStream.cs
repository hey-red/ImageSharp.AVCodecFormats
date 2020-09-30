using System;
using System.IO;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats.IO
{
    public sealed unsafe class AvioStream : IAvioStream
    {
        private readonly byte[] _readBuffer;

        private readonly Stream _inputStream;

        public AvioStream(Stream input)
        {
            _inputStream = input ?? throw new ArgumentNullException(nameof(input));
            _readBuffer = new byte[ReadBufferLength];
        }

        public int ReadBufferLength => 1024 * 8;

        // TODO: bench with span
        public int Read(void* opaque, byte* buffer, int bufferLength)
        {
            int readed = _inputStream.Read(_readBuffer, 0, _readBuffer.Length);
            if (readed > 0)
            {
                Marshal.Copy(_readBuffer, 0, (IntPtr)buffer, readed);
            }
            return readed;
        }

        public long Seek(void* opaque, long offset, int whence)
        {
            if (!_inputStream.CanSeek) return -1;

            return whence == ffmpeg.AVSEEK_SIZE ?
                _inputStream.Length :
                _inputStream.Seek(offset, SeekOrigin.Begin);
        }
    }
}