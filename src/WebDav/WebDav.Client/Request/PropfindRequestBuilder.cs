using System.Linq;
using System.Xml.Linq;
using WebDav.Helpers;

namespace WebDav.Request
{
    internal static class PropfindRequestBuilder
    {
        public static string BuildRequestBody(string[] customProperties)
        {
            XNamespace webDavNs = "DAV:";
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propfind = new XElement(webDavNs + "propfind", new XAttribute(XNamespace.Xmlns + "D", webDavNs));
            propfind.Add(new XElement(webDavNs + "allprop"));
            if (customProperties.Any())
            {
                var include = new XElement(webDavNs + "include");
                foreach (var prop in customProperties)
                {
                    include.Add(new XElement(webDavNs + prop));
                }
                propfind.Add(include);
            }
            doc.Add(propfind);
            return doc.ToStringWithDeclaration();
        }
    }
}
