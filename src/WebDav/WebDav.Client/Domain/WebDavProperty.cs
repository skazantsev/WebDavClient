using System.Xml.Linq;

namespace WebDav
{
    public class WebDavProperty
    {
        public XName Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}, Value: {1} }}", Name, Value);
        }
    }
}
