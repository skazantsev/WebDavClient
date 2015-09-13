using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WebDav.Exceptions;
using WebDav.Helpers;

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
                LockScope = PropertyParser.ParseLockScope(FindProp("lockscope", properties)),
                ApplyTo = PropertyParser.ParseLockDepth(FindProp("depth", properties)),
                Owner = PropertyParser.ParseOwner(FindProp("owner", properties)),
                Timeout = PropertyParser.ParseLockTimeout(FindProp("timeout", properties)),
                LockToken = PropertyParser.ParseString(FindProp("locktoken", properties)),
                ResourceHref = PropertyParser.ParseString(FindProp("lockroot", properties))
            };
            return activeLock;
        }

        private static XElement FindProp(string localName, IEnumerable<XElement> properties)
        {
            return properties.FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
