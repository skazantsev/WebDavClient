using System;

namespace WebDav
{
    public class ActiveLock
    {
        public ApplyTo.Lock ApplyTo { get; set; }

        public LockScope? LockScope { get; set; }

        public string LockToken { get; set; }

        public LockOwner Owner { get; set; }

        public string ResourceHref { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}
