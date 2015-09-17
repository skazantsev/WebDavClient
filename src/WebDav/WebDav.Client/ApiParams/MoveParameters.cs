using System.Threading;

namespace WebDav
{
    public class MoveParameters
    {
        public MoveParameters()
        {
            Overwrite = true;
            CancellationToken = CancellationToken.None;
        }

        public string SourceLockToken { get; set; }

        public string DestLockToken { get; set; }

        public bool Overwrite { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
