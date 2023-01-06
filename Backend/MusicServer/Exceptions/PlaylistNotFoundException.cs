using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class PlaylistNotFoundException : Exception
    {
        public PlaylistNotFoundException()
        {
        }

        public PlaylistNotFoundException(string? message) : base(message)
        {
        }

        public PlaylistNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PlaylistNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
