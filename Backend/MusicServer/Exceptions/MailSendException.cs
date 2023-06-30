using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class MailSendException : Exception
    {
        public MailSendException()
        {
        }

        public MailSendException(string? message) : base(message)
        {
        }

        public MailSendException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MailSendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
