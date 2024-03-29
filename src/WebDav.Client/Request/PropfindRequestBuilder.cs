﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class PropfindRequestBuilder
    {
        public static string BuildRequest(
            PropfindRequestType requestType,
            IReadOnlyCollection<XName> customProperties,
            IReadOnlyCollection<NamespaceAttr> namespaces)
        {
            return requestType == PropfindRequestType.NamedProperties
                ? BuildNamedPropRequest(customProperties, namespaces)
                : BuildAllPropRequest(customProperties, namespaces);
        }

        private static string BuildAllPropRequest(IReadOnlyCollection<XName> customProperties, IReadOnlyCollection<NamespaceAttr> namespaces)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propfind = new XElement(
                "{DAV:}propfind",
                new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
                new XElement("{DAV:}allprop")
            );
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

        private static string BuildNamedPropRequest(IReadOnlyCollection<XName> customProperties, IReadOnlyCollection<NamespaceAttr> namespaces)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var propfind = new XElement("{DAV:}propfind", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            if (customProperties.Any())
            {
                var propEl = new XElement("{DAV:}prop");
                foreach (var ns in namespaces)
                {
                    var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                    propEl.SetAttributeValue(nsAttr, ns.Namespace);
                }
                foreach (var prop in customProperties)
                {
                    propEl.Add(new XElement(prop));
                }
                propfind.Add(propEl);
            }
            doc.Add(propfind);
            return doc.ToStringWithDeclaration();
        }
    }
}
