using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using WebDav.Helpers;

namespace WebDav.Response
{
    internal static class ResourcePropertyParser
    {
        private static readonly IDictionary<string, ResourcePropertyType> DefaultProperties = DefaultResourceProperties.Get();

        public static object Parse(string localName, Func<string, XElement> propertyElementFinder)
        {
            var element = propertyElementFinder(localName);

            if (element == null)
                return null;

            switch (DefaultProperties[localName])
            {
                case ResourcePropertyType.String:
                    return element.Value;
                case ResourcePropertyType.Integer:
                    return ParseInteger(element.Value);
                case ResourcePropertyType.DateTime:
                    return ParseDateTime(element.Value);
                case ResourcePropertyType.ResourceType:
                    return ParseResourceType(element);
                default:
                    throw new InvalidEnumArgumentException("Wrong value for ResourcePropertyType.");
            }
        }

        private static object ParseInteger(string rawValue)
        {
            int value;
            return int.TryParse(rawValue, out value) ? (object) value : null;
        }

        private static object ParseDateTime(string rawValue)
        {
            DateTime value;
            return DateTime.TryParse(rawValue, out value) ? (object)value : null;
        }

        private static object ParseResourceType(XElement element)
        {
            return element.LocalNameElement("collection") != null
                ? ResourceType.Collection
                : ResourceType.Other;
        }
    }
}
