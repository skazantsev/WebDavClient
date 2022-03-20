using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class SearchRequestBuilder
    {
        /// <summary>
        /// Build the WEBDAV XML body.
        /// </summary>
        /// <param name="select">Set arguments to select parameters</param>
        /// <param name="fromDirectory">Set root directory to search from.</param>
        /// <param name="searchKeyWord">Set search keyword, e.g. 'test%' % = like.</param>
        /// <param name="where">Set argument where to search for.</param>
        /// <param name="namespaces">Optional, add namespaces.</param>
        /// <returns></returns>
        public static string BuildRequestBody(IReadOnlyCollection<XElement> select, string fromDirectory, string searchKeyWord, IReadOnlyCollection<XElement> where, IReadOnlyCollection<NamespaceAttr> namespaces = null)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var searchrequest = new XElement("{DAV:}searchrequest", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            if (!string.IsNullOrEmpty(fromDirectory))
            {
                var propBs = new XElement("{DAV:}basicsearch");
                foreach (var ns in namespaces)
                {
                    var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                    searchrequest.SetAttributeValue(nsAttr, ns.Namespace);
                }

                //Select part
                var propSelect = new XElement("{DAV:}select");
                var propSelectProp = new XElement("{DAV:}prop");

                foreach (var prop in select)
                {
                    propSelectProp.Add(new XElement(prop));
                }

                propSelect.Add(new XElement(propSelectProp));

                //From
                var propFrom = new XElement("{DAV:}from");
                var propFromScope = new XElement("{DAV:}scope");

                var propFromScopeHref = new XElement("{DAV:}href", fromDirectory);

                //Hardcoded depth for now.
                var propFromScopeDepth = new XElement("{DAV:}depth", "infinity");

                propFromScope.Add(new XElement(propFromScopeHref));
                propFromScope.Add(new XElement(propFromScopeDepth));

                propFrom.Add(new XElement(propFromScope));

                //Where
                var propWhere = new XElement("{DAV:}where");
                var propWhereLike = new XElement("{DAV:}like");
                var propWhereLikeProp = new XElement("{DAV:}prop");

                foreach (var prop in where)
                {
                    propWhereLikeProp.Add(new XElement(prop));
                }

                var propWhereLikeLiteral = new XElement("{DAV:}literal", searchKeyWord);

                propWhereLike.Add(new XElement(propWhereLikeProp));
                propWhereLike.Add(new XElement(propWhereLikeLiteral));
                propWhere.Add(new XElement(propWhereLike));

                propBs.Add(new XElement(propSelect));
                propBs.Add(new XElement(propFrom));
                propBs.Add(new XElement(propWhere));

                searchrequest.Add(propBs);
            }
            doc.Add(searchrequest);
            return doc.ToStringWithDeclaration();
        }
    }
}
