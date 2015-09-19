using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace WebDav
{
    public class PropfindParameters
    {
        public PropfindParameters()
        {
            CustomProperties = new List<XName>();
            Namespaces = new List<NamespaceAttr>();
            CancellationToken = CancellationToken.None;
        }

        public IReadOnlyCollection<XName> CustomProperties { get; set; }

        public IReadOnlyCollection<NamespaceAttr> Namespaces { get; set; }

        public ApplyTo.Propfind? ApplyTo { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
