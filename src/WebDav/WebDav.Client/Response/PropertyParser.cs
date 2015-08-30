using System;
using System.Xml.Linq;
using WebDav.Helpers;

namespace WebDav.Response
{
    internal static class PropertyParser
    {
        public static string ParseString(XElement element)
        {
            return element != null ? element.Value : null;
        }

        public static int? ParseInteger(XElement element)
        {
            if (element == null)
                return null;

            int value;
            return int.TryParse(element.Value, out value) ? (int?)value : null;
        }

        public static DateTime? ParseDateTime(XElement element)
        {
            if (element == null)
                return null;

            DateTime value;
            return DateTime.TryParse(element.Value, out value) ? (DateTime?)value : null;
        }

        public static ResourceType ParseResourceType(XElement element)
        {
            if (element == null)
                return ResourceType.Other;

            return element.LocalNameElement("collection") != null
                ? ResourceType.Collection
                : ResourceType.Other;
        }
    }
}
