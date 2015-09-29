using System.Xml.Linq;

namespace WebDav
{
    public class WebDavProperty
    {
        public WebDavProperty(XName name, string value)
        {
            Guard.NotNullOrEmpty((name ?? "").ToString(), "name");

            Name = name;
            Value = value;
        }

        public XName Name { get; private set; }

        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}, Value: {1} }}", Name, Value);
        }
    }
}
