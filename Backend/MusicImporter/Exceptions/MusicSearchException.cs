using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Exceptions
{
    public class MusicSearchException : Exception
    {
        public MusicSearchException()
        {
        }

        public MusicSearchException(string? message) : base(message)
        {
        }

        public MusicSearchException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MusicSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
