using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Response
{
    internal static class LockResponseParser
    {
        public static List<ActiveLock> Parse(string response)
        {
            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                throw new WebDavException("Failed to parse LOCK response.");

            var lockdiscovery = xresponse.Root.LocalNameElement("lockdiscovery", StringComparison.OrdinalIgnoreCase);
            return ParseLockDiscovery(lockdiscovery);
        }

        public static List<ActiveLock> ParseLockDiscovery(XElement lockdiscovery)
        {
            if (lockdiscovery == null)
                throw new WebDavException("Failed to parse LOCK response.");

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
