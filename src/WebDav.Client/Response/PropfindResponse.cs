﻿using System.Collections.Generic;
using System.Linq;

namespace WebDav
{
    /// <summary>
    /// Represents a response of the PROPFIND operation.
    /// </summary>
    public class PropfindResponse : WebDavResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropfindResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the operation.</param>
        public PropfindResponse(int statusCode)
            : this(statusCode, null, new List<WebDavResource>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropfindResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="resources">The collection of WebDAV resources.</param>
        public PropfindResponse(int statusCode, IEnumerable<WebDavResource> resources)
            : this(statusCode, null, resources)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropfindResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        public PropfindResponse(int statusCode, string? description)
            : this(statusCode, description, new List<WebDavResource>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropfindResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <param name="description">The description of the response.</param>
        /// <param name="resources">The collection of WebDAV resources.</param>
        public PropfindResponse(int statusCode, string? description, IEnumerable<WebDavResource> resources)
            : base(statusCode, description)
        {
            Guard.NotNull(resources, "resources");
            Resources = resources.ToList();
        }

        /// <summary>
        /// Gets the collection of WebDAV resources.
        /// </summary>
        public IReadOnlyCollection<WebDavResource> Resources { get; }

        public override string ToString()
        {
            return $"PROPFIND WebDAV response - StatusCode: {StatusCode}, Description: {Description}";
        }
    }
}
