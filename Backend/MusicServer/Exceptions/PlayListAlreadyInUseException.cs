using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class PlayListAlreadyInUseException : Exception
    {
        public PlayListAlreadyInUseException()
        {
        }

        public PlayListAlreadyInUseException(string? message) : base(message)
        {
        }

        public PlayListAlreadyInUseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PlayListAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
