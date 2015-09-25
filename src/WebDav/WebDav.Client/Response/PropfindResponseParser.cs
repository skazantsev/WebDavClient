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

        public static PropfindResponse Parse(string response, int statusCode, string description)
        {
            if (string.IsNullOrEmpty(response))
                return new PropfindResponse(statusCode, description);

            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                return new PropfindResponse(statusCode, description);

            var resources = xresponse.Root.LocalNameElements("response").Select(ParseResource).ToList();
            return new PropfindResponse(statusCode, description, resources);
        }

        private static WebDavResource ParseResource(XElement xres)
        {
            var hrefValue = xres.LocalNameElement("href", StringComparison.OrdinalIgnoreCase).GetValueOrNull();

            var proprestats = xres.LocalNameElements("propstat", StringComparison.OrdinalIgnoreCase)
                .Select(x => new { Propstat = x, StatusCode = GetStatusCodeForPropstat(x), Description = GetResponseDescription(x) }).ToList();

            var properties = proprestats.Where(x => IsSuccessStatusCode(x.StatusCode))
                .SelectMany(x => x.Propstat.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Elements())
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .ToList();

            var propertyErrors = proprestats.Where(x => !IsSuccessStatusCode(x.StatusCode))
                .SelectMany(x => x.Propstat.LocalNameElements("prop", StringComparison.OrdinalIgnoreCase)
                    .SelectMany(p => p.Elements())
                    .Select(p => new { Prop = p, StatusCode = x.StatusCode, Description = x.Description }))
                .GroupBy(x => x.Prop.Name)
                .Select(x => x.First())
                .ToDictionary(x => x.Prop.Name, v => new PropertyError(v.StatusCode, v.Description));

            return CreateResource(hrefValue, properties, propertyErrors);
        }

        private static WebDavResource CreateResource(string href, List<XElement> properties, Dictionary<XName, PropertyError> propertyErrors)
        {
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
                PropertyErrors = propertyErrors
            };

            if (resource.IsCollection)
                resource.Href = resource.Href.TrimEnd('/') + "/";
            return resource;
        }

        private static XElement FindProp(XName name, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.Equals(name));
        }

        private static int GetStatusCodeForPropstat(XElement propstatElement)
        {
            var statusRawValue = propstatElement.LocalNameElement("status", StringComparison.OrdinalIgnoreCase).GetValueOrNull();
            if (string.IsNullOrEmpty(statusRawValue))
                return default(int);

            var statusCodeGroup = StatusCodeRegex.Match(statusRawValue).Groups[1];
            if (!statusCodeGroup.Success)
                return default(int);

            int statusCode;
            if (!int.TryParse(statusCodeGroup.Value, out statusCode))
                return default(int);

            return statusCode;
        }

        private static string GetResponseDescription(XElement propstatElement)
        {
            return
                propstatElement.LocalNameElement("responsedescription", StringComparison.OrdinalIgnoreCase)
                    .GetValueOrNull();
        }

        private static bool IsSuccessStatusCode(int statusCode)
        {
            return statusCode >= 200 && statusCode <= 299;
        }
    }
}
