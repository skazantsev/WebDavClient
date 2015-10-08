using System.IO;

namespace WebDav
{
    /// <summary>
    /// Represents a response of the GET operation.
    /// </summary>
    public class WebDavStreamResponse : WebDavResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavStreamResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        public WebDavStreamResponse(int statusCode)
            : this(statusCode, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavStreamResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="stream">The stream of resource's content.</param>
        public WebDavStreamResponse(int statusCode, Stream stream)
            : this(statusCode, null, stream)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavStreamResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        public WebDavStreamResponse(int statusCode, string description)
            : this(statusCode, description, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavStreamResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        public WebDavStreamResponse(int statusCode, string description, Stream stream)
            : base(statusCode, description)
        {
            Stream = stream;
        }

        /// <summary>
        /// Gets the stream of content of the resource.
        /// </summary>
        public Stream Stream { get; private set; }

        public override string ToString()
        {
            return string.Format("WebDAV stream response - StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
