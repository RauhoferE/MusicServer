using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class ArtistNotFoundException : Exception
    {
        public ArtistNotFoundException()
        {
        }

        public ArtistNotFoundException(string? message) : base(message)
        {
        }

        public ArtistNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ArtistNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
