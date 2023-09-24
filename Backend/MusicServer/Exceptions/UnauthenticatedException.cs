using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException()
        {
        }

        public UnauthenticatedException(string? message) : base(message)
        {
        }

        public UnauthenticatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnauthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
