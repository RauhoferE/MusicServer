using System.Runtime.Serialization;

namespace MusicServer.Exceptions
{
    public class UploadedFileNotSupportedException : Exception
    {
        public UploadedFileNotSupportedException()
        {
        }

        public UploadedFileNotSupportedException(string? message) : base(message)
        {
        }

        public UploadedFileNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UploadedFileNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
