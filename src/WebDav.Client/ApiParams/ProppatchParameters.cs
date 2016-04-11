using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

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
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets the resource lock token.
        /// </summary>
        public string LockToken { get; set; }
    }
}
