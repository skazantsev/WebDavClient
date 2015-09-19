using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace WebDav
{
    public class ProppatchParameters
    {
        public ProppatchParameters()
        {
            PropertiesToSet = new Dictionary<XName, string>();
            PropertiesToRemove = new List<XName>();
            Namespaces = new List<NamespaceAttr>();
            CancellationToken = CancellationToken.None;
        }

        public IDictionary<XName, string> PropertiesToSet { get; set; }

        public IReadOnlyCollection<XName> PropertiesToRemove { get; set; }

        public IReadOnlyCollection<NamespaceAttr> Namespaces { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
