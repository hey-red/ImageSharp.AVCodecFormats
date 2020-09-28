using System;
using System.Runtime.Serialization;

namespace HeyRed.ImageSharp.AVCodecFormats
{
    public class AVException : Exception
    {
        public AVException()
        {
        }

        public AVException(string message) : base(message)
        {
        }

        public AVException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AVException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}