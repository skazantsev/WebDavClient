using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Request
{
    internal static class ProppatchRequestBuilder
    {
        public static string BuildRequestBody(IDictionary<string, string> propertiesToSet, IReadOnlyCollection<string> propertiesToRemove)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propertyupdate = new XElement("{DAV:}propertyupdate", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            if (propertiesToSet.Any())
            {
                var setEl = new XElement("{DAV:}set");
                foreach (var prop in propertiesToSet)
                {
                    var el = new XElement(XName.Get(prop.Key, "DAV:"));
                    el.SetInnerXml(prop.Value);
                    setEl.Add(new XElement(XName.Get("prop", "DAV:"), el));
                }
                propertyupdate.Add(setEl);
            }

            if (propertiesToRemove.Any())
            {
                var removeEl = new XElement("{DAV:}remove");
                foreach (var prop in propertiesToRemove)
                {
                    removeEl.Add(
                        new XElement(XName.Get("prop", "DAV:"),
                            new XElement(XName.Get(prop, "DAV:"))));
                }
                propertyupdate.Add(removeEl);
            }

            doc.Add(propertyupdate);
            return doc.ToStringWithDeclaration();
        }
    }
}
