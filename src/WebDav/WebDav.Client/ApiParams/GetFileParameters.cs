using System.Threading;

namespace WebDav
{
    public class GetFileParameters
    {
        public GetFileParameters()
        {
            CancellationToken = CancellationToken.None;
        }

        public CancellationToken CancellationToken { get; set; }
    }
}
