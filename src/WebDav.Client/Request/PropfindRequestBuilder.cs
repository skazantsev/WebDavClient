using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Request
{
    internal static class PropfindRequestBuilder
    {
        public static string BuildRequestBody(IReadOnlyCollection<XName> customProperties, IReadOnlyCollection<NamespaceAttr> namespaces)
        {            
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propfind = new XElement("{DAV:}propfind", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            propfind.Add(new XElement("{DAV:}allprop"));
            if (customProperties.Any())
            {
                var include = new XElement("{DAV:}include");
                foreach (var ns in namespaces)
                {
                    var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                    include.SetAttributeValue(nsAttr, ns.Namespace);
                }
                foreach (var prop in customProperties)
                {
                    include.Add(new XElement(prop));
                }
                propfind.Add(include);
            }
            doc.Add(propfind);
            return doc.ToStringWithDeclaration();
        }
    }
}
