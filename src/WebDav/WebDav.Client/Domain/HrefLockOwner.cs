using System;

namespace WebDav
{
    public class HrefLockOwner : LockOwner
    {
        private readonly string _href;

        public HrefLockOwner(string href)
        {
            if (!Uri.IsWellFormedUriString(href, UriKind.Absolute))
                throw new ArgumentException("The parameter href should a valid absolute uri.", "href");
            _href = href;
        }

        public override string Value
        {
            get { return _href; }
        }
    }
}
