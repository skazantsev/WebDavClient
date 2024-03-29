﻿using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class SearchRequestBuilder
    {
        public static string BuildRequestBody(SearchParameters parameters)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var search = new XElement("{DAV:}searchrequest", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));

            foreach (var ns in parameters.Namespaces)
            {
                var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                search.SetAttributeValue(nsAttr, ns.Namespace);
            }

            var basicSearch = new XElement(
                "{DAV:}basicsearch",
                BuildSelect(parameters),
                BuildFrom(parameters),
                BuildWhere(parameters)
            );
            search.Add(basicSearch);

            doc.Add(search);
            return doc.ToStringWithDeclaration();
        }

        private static XElement BuildSelect(SearchParameters parameters)
        {
            return new XElement(
                "{DAV:}select",
                parameters.SelectProperties.Any()
                    ? new XElement(
                        "{DAV:}prop",
                        parameters.SelectProperties.Select(prop => new XElement(prop)).ToArray()
                    )
                    : new XElement("{DAV:}allprop")
            );
        }

        private static XElement BuildFrom(SearchParameters parameters)
        {
            return new XElement(
                "{DAV:}from",
                new XElement(
                    "{DAV:}scope",
                    new XElement("{DAV:}href", parameters.Scope),
                    new XElement("{DAV:}depth", "infinity")
                )
            );
        }

        private static XElement BuildWhere(SearchParameters parameters)
        {
            return new XElement(
                "{DAV:}where",
                new XElement(
                    "{DAV:}like",
                    new XElement("{DAV:}prop", new XElement(parameters.SearchProperty)),
                    new XElement("{DAV:}literal", parameters.SearchKeyword)
                )
            );
        }
    }
}
