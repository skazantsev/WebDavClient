using System.Collections.Generic;

namespace WebDav
{
    public class ProppatchResponse : WebDavResponse
    {
        public ProppatchResponse(int statusCode)
            : this(statusCode, null, new List<WebDavPropertyStatus>())
        {
        }

        public ProppatchResponse(int statusCode, IEnumerable<WebDavPropertyStatus> propertyStatuses)
            : this(statusCode, null, propertyStatuses)
        {
        }

        public ProppatchResponse(int statusCode, string description)
            : this(statusCode, description, new List<WebDavPropertyStatus>())
        {
        }

        public ProppatchResponse(int statusCode, string description, IEnumerable<WebDavPropertyStatus> propertyStatuses)
            : base(statusCode, description)
        {
            Guard.NotNull(propertyStatuses, "propertyStatuses");
            PropertyStatuses = new List<WebDavPropertyStatus>(propertyStatuses);
        }

        public IReadOnlyCollection<WebDavPropertyStatus> PropertyStatuses { get; set; }

        public override string ToString()
        {
            return string.Format("PROPPATCH WebDav response - StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
