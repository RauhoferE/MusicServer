using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class UserAlreadyAssignedException : Exception
    {
        public UserAlreadyAssignedException()
        {
        }

        public UserAlreadyAssignedException(string? message) : base(message)
        {
        }

        public UserAlreadyAssignedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserAlreadyAssignedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
