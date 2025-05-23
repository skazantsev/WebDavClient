﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav
{
    internal static class LinqToXmlExtentions
    {
        public static string ToStringWithDeclaration(this XDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return doc.Declaration + Environment.NewLine + doc;
        }

        public static string? GetValueOrNull(this XElement element)
        {
            return element?.Value;
        }

        public static XElement LocalNameElement(this XElement parent, string localName)
        {
            return LocalNameElement(parent, localName, StringComparison.Ordinal);
        }

        public static XElement LocalNameElement(this XElement parent, string localName, StringComparison comparisonType)
        {
            return parent.Elements().FirstOrDefault(e => e.Name.LocalName.Equals(localName, comparisonType));
        }

        public static IEnumerable<XElement> LocalNameElements(this XElement parent, string localName)
        {
            return LocalNameElements(parent, localName, StringComparison.Ordinal);
        }

        public static IEnumerable<XElement> LocalNameElements(this XElement parent, string localName, StringComparison comparisonType)
        {
            return parent.Elements().Where(e => e.Name.LocalName.Equals(localName, comparisonType));
        }

        public static string GetInnerXml(this XElement element)
        {
            using (var reader = element.CreateReader())
            {
                reader.MoveToContent();
                return reader.ReadInnerXml();
            }
        }

        public static void SetInnerXml(this XElement element, string innerXml)
        {
            element.ReplaceNodes(XElement.Parse("<dummy>" + innerXml + "</dummy>").Nodes());
        }
    }
}
