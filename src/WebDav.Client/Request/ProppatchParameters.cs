﻿using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Xml.Linq;
using WebDav.Client.Core;

namespace WebDav
{
    /// <summary>
    /// Represents parameters for the PROPPATCH WebDAV method.
    /// </summary>
    public class ProppatchParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProppatchParameters"/> class.
        /// </summary>
        public ProppatchParameters()
        {
            PropertiesToSet = new Dictionary<XName, string>();
            PropertiesToRemove = new List<XName>();
            Namespaces = new List<NamespaceAttr>();
            ContentType = MediaTypes.XmlMediaType;
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets properties to set on the resource.
        /// </summary>
        public IDictionary<XName, string> PropertiesToSet { get; set; }

        /// <summary>
        /// Gets or sets the collection of properties defined on the resource to remove.
        /// </summary>
        public IReadOnlyCollection<XName> PropertiesToRemove { get; set; }

        /// <summary>
        /// Gets or sets the collection of xml namespaces of properties.
        /// </summary>
        public IReadOnlyCollection<NamespaceAttr> Namespaces { get; set; }

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

        /// <summary>
        /// Gets or sets the resource lock token.
        /// </summary>
        public string? LockToken { get; set; }
    }
}
