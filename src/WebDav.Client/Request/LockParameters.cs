﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using WebDav.Client.Core;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the LOCK WebDAV method.
    /// </summary>
    public class LockParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockParameters"/> class.
        /// </summary>
        public LockParameters()
        {
            ContentType = MediaTypes.XmlMediaType;
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this lock is an exclusive lock or a shared lock.
        /// </summary>
        public LockScope LockScope { get; set; }

        /// <summary>
        /// Gets or sets the timeout of this lock.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the method is to be applied only to the resource or the resource and all its members.
        /// It corresponds to the WebDAV Depth header.
        /// </summary>
        public ApplyTo.Lock? ApplyTo { get; set; }

        /// <summary>
        /// Gets or sets the owner of this lock.
        /// </summary>
        public LockOwner? Owner { get; set; }

        /// <summary>
        /// Gets or sets the content type of the request body.
        /// The default value is application/xml; charset=utf-8.
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; set; }

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
