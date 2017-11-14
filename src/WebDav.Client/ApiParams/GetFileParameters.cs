using System;
using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the GET WebDAV method.
    /// </summary>
    public class GetFileParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetFileParameters"/> class.
        /// </summary>
        public GetFileParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the quoted ETag string use by the If-None-Match HTTP header.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the value of the If-Modified-Since HTTP header.
        /// </summary>
        public DateTimeOffset? IfModifiedSince { get; set; }
    }
}
