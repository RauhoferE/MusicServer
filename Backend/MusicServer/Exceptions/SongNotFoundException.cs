using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class SongNotFoundException : Exception
    {
        public SongNotFoundException()
        {
        }

        public SongNotFoundException(string? message) : base(message)
        {
        }

        public SongNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SongNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
