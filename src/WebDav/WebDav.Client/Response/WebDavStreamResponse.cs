using System.IO;

namespace WebDav
{
    public class WebDavStreamResponse : WebDavResponse
    {
        public WebDavStreamResponse(int statusCode)
            : this(statusCode, null, null)
        {
        }

        public WebDavStreamResponse(int statusCode, Stream stream)
            : this(statusCode, null, stream)
        {
        }

        public WebDavStreamResponse(int statusCode, string description)
            : this(statusCode, description, null)
        {
        }

        public WebDavStreamResponse(int statusCode, string description, Stream stream)
            : base(statusCode, description)
        {
            Stream = stream;
        }

        public Stream Stream { get; private set; }

        public override string ToString()
        {
            return string.Format("WebDav stream response - StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
