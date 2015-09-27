using System.Xml.Linq;

namespace WebDav
{
    public class WebDavPropertyStatus
    {
        public XName Name { get; set; }

        public int StatusCode { get; set; }

        public string Description { get; set; }

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
