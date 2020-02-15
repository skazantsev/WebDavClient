using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class ProppatchRequestBuilder
    {
        public static string BuildRequestBody(IDictionary<XName, string> propertiesToSet,
            IReadOnlyCollection<XName> propertiesToRemove,
            IReadOnlyCollection<NamespaceAttr> namespaces)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propertyupdate = new XElement("{DAV:}propertyupdate", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            foreach (var ns in namespaces)
            {
                var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                propertyupdate.SetAttributeValue(nsAttr, ns.Namespace);
            }
            if (propertiesToSet.Any())
            {
                var setEl = new XElement("{DAV:}set");
                var propEl = new XElement("{DAV:}prop");
                foreach (var prop in propertiesToSet)
                {
                    var el = new XElement(prop.Key);
                    el.SetInnerXml(prop.Value);
                    propEl.Add(el);
                }
                setEl.Add(propEl);
                propertyupdate.Add(setEl);
            }

            if (propertiesToRemove.Any())
            {
                var removeEl = new XElement("{DAV:}remove");
                var propEl = new XElement("{DAV:}prop");
                foreach (var prop in propertiesToRemove)
                {
                    propEl.Add(new XElement(prop));
                }
                removeEl.Add(propEl);
                propertyupdate.Add(removeEl);
            }

            doc.Add(propertyupdate);
            return doc.ToStringWithDeclaration();
        }
    }
}
