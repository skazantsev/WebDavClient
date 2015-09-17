using System.Collections.Generic;
using System.Threading;

namespace WebDav
{
    public class PropfindParameters
    {
        public PropfindParameters()
        {
            CustomProperties = new List<string>();
            CancellationToken = CancellationToken.None;
        }

        public IReadOnlyCollection<string> CustomProperties { get; set; }

        public ApplyTo.Propfind? ApplyTo { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
