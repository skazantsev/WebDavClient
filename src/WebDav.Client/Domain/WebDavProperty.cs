using System.Xml.Linq;

namespace WebDav
{
    /// <summary>
    /// Represents a WebDAV resource property.
    /// </summary>
    public class WebDavProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavProperty"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public WebDavProperty(XName name, string value)
        {
            Guard.NotNullOrEmpty((name ?? "").ToString(), "name");

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public XName Name { get; private set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}, Value: {1} }}", Name, Value);
        }
    }
}
