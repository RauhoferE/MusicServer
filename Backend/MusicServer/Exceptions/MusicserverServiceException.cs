using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class MusicserverServiceException : Exception
    {
        public MusicserverServiceException()
        {
        }

        public MusicserverServiceException(string? message) : base(message)
        {
        }

        public MusicserverServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MusicserverServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
