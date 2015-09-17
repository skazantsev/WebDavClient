using System.Threading;

namespace WebDav
{
    public class PutFileParameters
    {
        public PutFileParameters()
        {
            ContentType = "application/octet-stream";
            CancellationToken = CancellationToken.None;
        }

        public string ContentType { get; set; }

        public string LockToken { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
