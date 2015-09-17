using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WebDav.Response
{
    internal static class PropfindResponseParser
    {
        private static readonly Regex StatusCodeRegex = new Regex(@".*(\d{3}).*");

        public static PropfindResponse Parse(string response)
        {
            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                throw new WebDavException("Failed to parse PROPFIND response.");

            return new PropfindResponse
            {
                Resources =
                    xresponse.Root.LocalNameElements("response")
                        .Select(ParseResource)
                        .ToList()
            };
        }

        private static WebDavResource ParseResource(XElement xres)
        {
            var hrefValue = xres.LocalNameElement("href", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
            var properties = xres.LocalNameElements("propstat", StringComparison.OrdinalIgnoreCase)
                .Where(IsSuccessStatusCode)
                .SelectMany(x => x.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Elements())
                .ToList();

            return CreateResource(hrefValue, properties);
        }

        private static WebDavResource CreateResource(string href, List<XElement> properties)
        {
            var resource = new WebDavResource
            {
                ActiveLocks = LockResponseParser.ParseLockDiscovery(FindProp("lockdiscovery", properties)),
                
                ContentLanguage = PropertyParser.ParseString(FindProp("getcontentlanguage", properties)),
                ContentLength = PropertyParser.ParseInteger(FindProp("getcontentlength", properties)),
                ContentType = PropertyParser.ParseString(FindProp("getcontenttype", properties)),
                CreationDate = PropertyParser.ParseDateTime(FindProp("creationdate", properties)),
                DisplayName = PropertyParser.ParseString(FindProp("displayname", properties)),
                ETag = PropertyParser.ParseString(FindProp("getetag", properties)),
                Href = href,
                IsCollection = PropertyParser.ParseInteger(FindProp("iscollection", properties)) > 0 ||
                    PropertyParser.ParseResourceType(FindProp("resourcetype", properties)) == ResourceType.Collection,
                IsHidden = PropertyParser.ParseInteger(FindProp("ishidden", properties)) > 0,
                LastModifiedDate = PropertyParser.ParseDateTime(FindProp("getlastmodified", properties)),
                Properties = properties.ToDictionary(k => k.Name.LocalName, v => v.GetInnerXml())
            };

            if (resource.IsCollection)
                resource.Href = resource.Href.TrimEnd('/') + "/";
            return resource;
        }

        private static XElement FindProp(string localName, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsSuccessStatusCode(XElement propstatElement)
        {
            var statusRawValue = propstatElement.LocalNameElement("status", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
            if (string.IsNullOrEmpty(statusRawValue))
                return false;

            var statusCodeGroup = StatusCodeRegex.Match(statusRawValue).Groups[1];
            if (!statusCodeGroup.Success)
                return false;

            int statusCode;
            if (!int.TryParse(statusCodeGroup.Value, out statusCode))
                return false;

            return statusCode >= 200 && statusCode <= 299;
        }
    }
}
