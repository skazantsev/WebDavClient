﻿using System.Collections.Generic;
using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the DELETE WebDAV method.
    /// </summary>
    public class DeleteParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteParameters"/> class.
        /// </summary>
        public DeleteParameters()
        {
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

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
