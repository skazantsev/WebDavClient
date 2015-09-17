using System.Threading;

namespace WebDav
{
    public class MkColParameters
    {
        public MkColParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        public string LockToken { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
