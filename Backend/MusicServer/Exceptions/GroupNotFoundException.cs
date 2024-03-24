using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class GroupNotFoundException : Exception
    {
        public GroupNotFoundException()
        {
        }

        public GroupNotFoundException(string? message) : base(message)
        {
        }

        public GroupNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected GroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
