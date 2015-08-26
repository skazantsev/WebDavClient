using System;

namespace WebDav
{
    public class WebDavResource
    {
        public bool IsCollection { get; set; }

        public string Href { get; set; }

        public DateTime? CreationDate { get; set; }

        public string DisplayName { get; set; }

        public string ContentLanguage { get; set; }

        public int? ContentLength { get; set; }

        public string ContentType { get; set; }

        public string ETag { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
