using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the PUT WebDAV method.
    /// </summary>
    public class PutFileParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PutFileParameters"/> class.
        /// </summary>
        public PutFileParameters()
        {
            ContentType = "application/octet-stream";
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the content type of the request body.
        /// The default value is application/octet-stream.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the resource lock token.
        /// </summary>
        public string LockToken { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
