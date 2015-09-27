using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Response
{
    internal static class PropfindResponseParser
    {
        public static PropfindResponse Parse(string response, int statusCode, string description)
        {
            if (string.IsNullOrEmpty(response))
                return new PropfindResponse(statusCode, description);

            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                return new PropfindResponse(statusCode, description);

            var resources = xresponse.Root.LocalNameElements("response", StringComparison.OrdinalIgnoreCase)
                .Select(ParseResource)
                .ToList();
            return new PropfindResponse(statusCode, description, resources);
        }

        private static WebDavResource ParseResource(XElement xresponse)
        {
            var hrefValue = xresponse.LocalNameElement("href", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
            var propstats = MultiStatusParser.GetPropstats(xresponse);
            return CreateResource(hrefValue, propstats);
        }

        private static WebDavResource CreateResource(string href, List<MultiStatusParser.Propstat> propstats)
        {
            var properties = MultiStatusParser.GetProperties(propstats);
            var resource = new WebDavResource
            {
                ActiveLocks = LockResponseParser.ParseLockDiscovery(FindProp("{DAV:}lockdiscovery", properties)),
                ContentLanguage = PropertyValueParser.ParseString(FindProp("{DAV:}getcontentlanguage", properties)),
                ContentLength = PropertyValueParser.ParseInteger(FindProp("{DAV:}getcontentlength", properties)),
                ContentType = PropertyValueParser.ParseString(FindProp("{DAV:}getcontenttype", properties)),
                CreationDate = PropertyValueParser.ParseDateTime(FindProp("{DAV:}creationdate", properties)),
                DisplayName = PropertyValueParser.ParseString(FindProp("{DAV:}displayname", properties)),
                ETag = PropertyValueParser.ParseString(FindProp("{DAV:}getetag", properties)),
                Href = href,
                IsCollection = PropertyValueParser.ParseInteger(FindProp("{DAV:}iscollection", properties)) > 0 ||
                    PropertyValueParser.ParseResourceType(FindProp("{DAV:}resourcetype", properties)) == ResourceType.Collection,
                IsHidden = PropertyValueParser.ParseInteger(FindProp("{DAV:}ishidden", properties)) > 0,
                LastModifiedDate = PropertyValueParser.ParseDateTime(FindProp("{DAV:}getlastmodified", properties)),
                Properties = properties.ToDictionary(k => k.Name, v => v.GetInnerXml()),
                PropertyStatuses = MultiStatusParser.GetPropertyStatuses(propstats)
            };

            if (resource.IsCollection)
                resource.Href = resource.Href.TrimEnd('/') + "/";
            return resource;
        }

        private static XElement FindProp(XName name, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.Equals(name));
        }
    }
}
