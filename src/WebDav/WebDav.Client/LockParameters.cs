using System;

namespace WebDav
{
    public class LockParameters
    {
        public LockScope LockScope { get; set; }

        public TimeSpan? Timeout { get; set; }

        public ApplyTo.Lock? ApplyTo { get; set; }

        public LockOwner Owner { get; set; }
    }
}
