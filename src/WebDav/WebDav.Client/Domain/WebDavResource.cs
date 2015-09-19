using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace WebDav
{
    public class WebDavResource
    {
        public WebDavResource()
        {
            Properties = new Dictionary<XName, string>();
        }

        public List<ActiveLock> ActiveLocks { get; set; }

        public string ContentLanguage { get; set; }

        public int? ContentLength { get; set; }

        public string ContentType { get; set; }

        public DateTime? CreationDate { get; set; }

        public string DisplayName { get; set; }

        public string ETag { get; set; }

        public string Href { get; set; }

        public bool IsCollection { get; set; }

        public bool IsHidden { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public Dictionary<XName, string> Properties { get; set; }
    }
}
