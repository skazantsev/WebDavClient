using System;
using System.Threading;

namespace WebDav
{
    public class LockParameters
    {
        public LockParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        public LockScope LockScope { get; set; }

        public TimeSpan? Timeout { get; set; }

        public ApplyTo.Lock? ApplyTo { get; set; }

        public LockOwner Owner { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
