using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Response
{
    internal static class LockResponseParser
    {
        public static LockResponse Parse(string response, int statusCode, string description)
        {
            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                return new LockResponse(statusCode, description);

            var lockdiscovery = xresponse.Root.LocalNameElement("lockdiscovery", StringComparison.OrdinalIgnoreCase);
            var activeLocks = ParseLockDiscovery(lockdiscovery);
            return new LockResponse(statusCode, description, activeLocks);
        }

        public static List<ActiveLock> ParseLockDiscovery(XElement lockdiscovery)
        {
            if (lockdiscovery == null)
                return new List<ActiveLock>();

            return lockdiscovery
                .LocalNameElements("activelock", StringComparison.OrdinalIgnoreCase)
                .Select(x => CreateActiveLock(x.Elements().ToList()))
                .ToList();
        }

        private static ActiveLock CreateActiveLock(List<XElement> properties)
        {
            var activeLock = new ActiveLock
            {
                ApplyTo = PropertyValueParser.ParseLockDepth(FindProp("depth", properties)),
                LockScope = PropertyValueParser.ParseLockScope(FindProp("lockscope", properties)),
                LockToken = PropertyValueParser.ParseString(FindProp("locktoken", properties)),
                Owner = PropertyValueParser.ParseOwner(FindProp("owner", properties)),
                ResourceHref = PropertyValueParser.ParseString(FindProp("lockroot", properties)),
                Timeout = PropertyValueParser.ParseLockTimeout(FindProp("timeout", properties))
            };
            return activeLock;
        }

        private static XElement FindProp(string localName, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
