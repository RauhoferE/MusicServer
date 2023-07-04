using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class MessageTypeNotFoundException : Exception
    {
        public MessageTypeNotFoundException()
        {
        }

        public MessageTypeNotFoundException(string? message) : base(message)
        {
        }

        public MessageTypeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MessageTypeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
