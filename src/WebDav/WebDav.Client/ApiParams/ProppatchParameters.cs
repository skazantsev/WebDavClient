using System.Collections.Generic;
using System.Threading;

namespace WebDav
{
    public class ProppatchParameters
    {
        public ProppatchParameters()
        {
            PropertiesToSet = new Dictionary<string, string>();
            PropertiesToRemove = new List<string>();
            CancellationToken = CancellationToken.None;
        }

        public IDictionary<string, string> PropertiesToSet { get; set; }

        public IReadOnlyCollection<string> PropertiesToRemove { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
