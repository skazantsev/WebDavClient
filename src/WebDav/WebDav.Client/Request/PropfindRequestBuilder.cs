using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Request
{
    internal static class PropfindRequestBuilder
    {
        public static string BuildRequestBody(IReadOnlyCollection<string> customProperties)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propfind = new XElement("{DAV:}propfind", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            propfind.Add(new XElement("{DAV:}allprop"));
            if (customProperties.Any())
            {
                var include = new XElement("{DAV:}include");
                foreach (var prop in customProperties)
                {
                    include.Add(new XElement(XName.Get(prop, "DAV:")));
                }
                propfind.Add(include);
            }
            doc.Add(propfind);
            return doc.ToStringWithDeclaration();
        }
    }
}
