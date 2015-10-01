using System;

namespace WebDav
{
    public class UriLockOwner : LockOwner
    {
        private readonly string _uri;

        public UriLockOwner(string absoluteUri)
        {
            if (!Uri.IsWellFormedUriString(absoluteUri, UriKind.Absolute))
                throw new ArgumentException("The parameter uri should a valid absolute uri.", "absoluteUri");
            _uri = absoluteUri;
        }

        public override string Value
        {
            get { return _uri; }
        }
    }
}
