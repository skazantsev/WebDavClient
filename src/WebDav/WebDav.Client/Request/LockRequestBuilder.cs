using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace WebDav.Request
{
    internal static class LockRequestBuilder
    {
        public static string BuildRequestBody(LockParameters lockParams)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var lockinfo = new XElement("{DAV:}lockinfo", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
            lockinfo.Add(GetLockScope(lockParams.LockScope));
            lockinfo.Add(GetLockType());
            if (lockParams.Owner != null)
                lockinfo.Add(GetLockOwner(lockParams.Owner));


            doc.Add(lockinfo);
            return doc.ToStringWithDeclaration();
        }

        private static XElement GetLockScope(LockScope lockScope)
        {
            var lockscope = new XElement("{DAV:}lockscope");
            switch (lockScope)
            {
                case LockScope.Shared:
                    lockscope.Add(new XElement("{DAV:}shared"));
                    break;
                case LockScope.Exclusive:
                    lockscope.Add(new XElement("{DAV:}exclusive"));
                    break;
                default:
                    throw new InvalidEnumArgumentException("lockScope", (int)lockScope, typeof(LockScope));
            }
            return lockscope;
        }

        private static XElement GetLockType()
        {
            var locktype = new XElement("{DAV:}locktype");
            locktype.Add(new XElement("{DAV:}write"));
            return locktype;
        }

        private static XElement GetLockOwner(LockOwner lockOwner)
        {
            var owner = new XElement("{DAV:}owner");
            if (lockOwner is PrincipalLockOwner)
            {
                owner.SetValue(lockOwner.Value);
            }
            else if (lockOwner is UriLockOwner)
            {
                var uri = new XElement("{DAV:}href");
                uri.SetValue(lockOwner.Value);
                owner.Add(uri);
            }
            else
            {
                throw new ArgumentException("Lock owner is invalid.", "lockOwner");
            }
            return owner;
        }
    }
}
