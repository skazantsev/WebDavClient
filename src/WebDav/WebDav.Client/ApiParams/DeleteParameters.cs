using System.Threading;

namespace WebDav
{
    public class DeleteParameters
    {
        public DeleteParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        public string LockToken { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
