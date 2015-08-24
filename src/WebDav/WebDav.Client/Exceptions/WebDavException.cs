using System;
using System.Runtime.Serialization;

namespace WebDav.Exceptions
{
    public class WebDavException : Exception
    {
        public WebDavException()
        {
        }

        public int HttpCode { get; private set; }

        public WebDavException(string message)
            : base(message)
        {
        }

        public WebDavException(int httpCode, string message)
            : base(message)
        {
            HttpCode = httpCode;
        }

        public WebDavException(int httpCode, string message, Exception innerException)
            : base(message, innerException)
        {
            HttpCode = httpCode;
        }

        public WebDavException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override string ToString()
        {
            return string.Format("Http code: {0}.{1}{2}", HttpCode, Environment.NewLine, base.ToString());
        }
    }
}
