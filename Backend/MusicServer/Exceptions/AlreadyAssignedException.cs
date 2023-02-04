using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class AlreadyAssignedException : Exception
    {
        public AlreadyAssignedException()
        {
        }

        public AlreadyAssignedException(string? message) : base(message)
        {
        }

        public AlreadyAssignedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AlreadyAssignedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
