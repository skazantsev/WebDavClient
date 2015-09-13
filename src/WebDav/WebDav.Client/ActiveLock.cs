using System;

namespace WebDav
{
    public class ActiveLock
    {
        public string ResourceHref { get; set; }

        public string LockToken { get; set; }

        public LockScope? LockScope { get; set; }

        public ApplyTo.Lock ApplyTo { get; set; }

        public LockOwner Owner { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}
