using System;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    internal static class AVErrorHelper
    {
        public static unsafe string AVStrError(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int error)
        {
            if (error < 0) throw new AVException(AVStrError(error));
            return error;
        }
    }
}