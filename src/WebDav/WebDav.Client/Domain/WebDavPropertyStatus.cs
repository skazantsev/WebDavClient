using System.Xml.Linq;

namespace WebDav
{
    public class WebDavPropertyStatus
    {
        public WebDavPropertyStatus(XName name, int statusCode)
            :this (name, statusCode, null)
        {
        }

        public WebDavPropertyStatus(XName name, int statusCode, string description)
        {
            Guard.NotNullOrEmpty((name ?? "").ToString(), "name");

            Name = name;
            StatusCode = statusCode;
            Description = description;
        }

        public XName Name { get; private set; }

        public int StatusCode { get;  private set; }

        public string Description { get; private set; }

        public virtual bool IsSuccessful
        {
            get { return StatusCode >= 200 && StatusCode <= 299; }
        }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}, StatusCode: {1}, Description: {2} }}", Name, StatusCode, Description);
        }
    }
}
