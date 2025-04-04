﻿using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using WebDav.Client.Core;

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
            ContentType = MediaTypes.BinaryDataMediaType;
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the content type of the request body.
        /// The default value is application/octet-stream.
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; set; }

        /// <summary>
        /// Gets or sets the resource lock token.
        /// </summary>
        public string? LockToken { get; set; }

        /// <summary>
        /// Gets or sets the collection of http request headers.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
