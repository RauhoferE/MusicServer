using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Exceptions
{
    public class SongExistsException : Exception
    {
        public SongExistsException()
        {
        }

        public SongExistsException(string? message) : base(message)
        {
        }

        public SongExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SongExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
