using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class AlbumNotFoundException : Exception
    {
        public AlbumNotFoundException()
        {
        }

        public AlbumNotFoundException(string? message) : base(message)
        {
        }

        public AlbumNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AlbumNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
