using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebDav.Client.Request;

namespace WebDav
{
    internal static class SearchRequestBuilder
    {
        /// <summary>
        /// Build the WEBDAV XML body.
        /// </summary>
        /// <param name="parameters">Set the SEARCH requests parameters</param>
        /// <returns></returns>
        public static string BuildRequestBody(SearchParameters parameters)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var searchrequest = new XElement("{DAV:}searchrequest", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            if (!string.IsNullOrEmpty(parameters.SearchPath))
            {
                var propBs = new XElement("{DAV:}basicsearch");
                foreach (var ns in parameters.Namespaces)
                {
                    var nsAttr = string.IsNullOrEmpty(ns.Prefix) ? "xmlns" : XNamespace.Xmlns + ns.Prefix;
                    searchrequest.SetAttributeValue(nsAttr, ns.Namespace);
                }

                //Select part
                var propSelect = new XElement("{DAV:}select");
                var propSelectProp = new XElement("{DAV:}prop");

                foreach (var prop in parameters.SelectProperties)
                {
                    propSelectProp.Add(new XElement(prop));
                }

                propSelect.Add(new XElement(propSelectProp));

                //From
                var propFrom = new XElement("{DAV:}from");
                var propFromScope = new XElement("{DAV:}scope");

                var propFromScopeHref = new XElement("{DAV:}href", parameters.SearchPath);

                //Hardcoded depth for now.
                var propFromScopeDepth = new XElement("{DAV:}depth", "infinity");

                propFromScope.Add(new XElement(propFromScopeHref));
                propFromScope.Add(new XElement(propFromScopeDepth));

                propFrom.Add(new XElement(propFromScope));

                //Where
                var propWhere = new XElement("{DAV:}where");
                var propWhereLike = new XElement("{DAV:}like");
                var propWhereLikeProp = new XElement("{DAV:}prop");

                foreach (var prop in parameters.WhereProperties)
                {
                    propWhereLikeProp.Add(new XElement(prop));
                }

                var propWhereLikeLiteral = new XElement("{DAV:}literal", parameters.SearchKeyword);

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
