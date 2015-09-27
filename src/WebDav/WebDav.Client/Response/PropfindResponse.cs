using System.Collections.Generic;
using System.Linq;

namespace WebDav
{
    public class PropfindResponse : WebDavResponse
    {
        public PropfindResponse(int statusCode)
            : this(statusCode, null, new List<WebDavResource>())
        {
        }

        public PropfindResponse(int statusCode, IEnumerable<WebDavResource> resources)
            : this(statusCode, null, resources)
        {
        }

        public PropfindResponse(int statusCode, string description)
            : this(statusCode, description, new List<WebDavResource>())
        {
        }

        public PropfindResponse(int statusCode, string description, IEnumerable<WebDavResource> resources)
            : base(statusCode, description)
        {
            Guard.NotNull(resources, "resources");
            Resources = resources.ToList();
        }

        public IReadOnlyCollection<WebDavResource> Resources { get; private set; }

        public override string ToString()
        {
            return string.Format("PROPFIND WebDav response - StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
