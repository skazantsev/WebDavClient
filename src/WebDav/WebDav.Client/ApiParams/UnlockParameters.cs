using System.Threading;

namespace WebDav
{
    public class UnlockParameters
    {
        public UnlockParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        public string LockToken { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
