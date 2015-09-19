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
                ActiveLocks = LockResponseParser.ParseLockDiscovery(FindProp("{DAV:}lockdiscovery", properties)),
                
                ContentLanguage = PropertyParser.ParseString(FindProp("{DAV:}getcontentlanguage", properties)),
                ContentLength = PropertyParser.ParseInteger(FindProp("{DAV:}getcontentlength", properties)),
                ContentType = PropertyParser.ParseString(FindProp("{DAV:}getcontenttype", properties)),
                CreationDate = PropertyParser.ParseDateTime(FindProp("{DAV:}creationdate", properties)),
                DisplayName = PropertyParser.ParseString(FindProp("{DAV:}displayname", properties)),
                ETag = PropertyParser.ParseString(FindProp("{DAV:}getetag", properties)),
                Href = href,
                IsCollection = PropertyParser.ParseInteger(FindProp("{DAV:}iscollection", properties)) > 0 ||
                    PropertyParser.ParseResourceType(FindProp("{DAV:}resourcetype", properties)) == ResourceType.Collection,
                IsHidden = PropertyParser.ParseInteger(FindProp("{DAV:}ishidden", properties)) > 0,
                LastModifiedDate = PropertyParser.ParseDateTime(FindProp("{DAV:}getlastmodified", properties)),
                Properties = properties.ToDictionary(k => k.Name, v => v.GetInnerXml())
            };

            if (resource.IsCollection)
                resource.Href = resource.Href.TrimEnd('/') + "/";
            return resource;
        }

        private static XElement FindProp(XName name, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.Equals(name));
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
