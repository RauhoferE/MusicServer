using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class AuthenticationServiceException : Exception
    {
        public AuthenticationServiceException()
        {
        }

        public AuthenticationServiceException(string? message) : base(message)
        {
        }

        public AuthenticationServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AuthenticationServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
