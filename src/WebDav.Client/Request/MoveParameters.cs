﻿using System.Collections.Generic;
using System.Threading;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the MOVE WebDAV method.
    /// </summary>
    public class MoveParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveParameters"/> class.
        /// </summary>
        public MoveParameters()
        {
            Overwrite = true;
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets the lock token for the moving resource.
        /// </summary>
        public string? SourceLockToken { get; set; }

        /// <summary>
        /// Gets or sets the lock token for the destination resource.
        /// </summary>
        public string? DestLockToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the server should overwrite a non-null destination.
        /// For more information see https://msdn.microsoft.com/en-us/library/aa142944.
        /// </summary>
        /// <value>
        /// <c>true</c> if the the server should overwrite a non-null destination; otherwise <c>false</c>. The default value is <c>true</c>.
        /// </value>
        public bool Overwrite { get; set; }

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
