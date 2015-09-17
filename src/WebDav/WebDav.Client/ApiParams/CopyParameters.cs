using System.Threading;

namespace WebDav
{
    public class CopyParameters
    {
        public CopyParameters()
        {
            Overwrite = true;
            CancellationToken = CancellationToken.None;
        }

        public ApplyTo.Copy? ApplyTo { get; set; }

        public string DestLockToken { get; set; }

        public bool Overwrite { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
