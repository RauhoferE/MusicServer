using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class UserSessionException : Exception
    {
        public UserSessionException()
        {
        }

        public UserSessionException(string? message) : base(message)
        {
        }

        public UserSessionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserSessionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
