using System.Xml.Linq;

namespace WebDav
{
    /// <summary>
    /// Represents a status of an operation on a resource property.
    /// </summary>
    public class WebDavPropertyStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavPropertyStatus"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="statusCode">The status code of the operation.</param>
        public WebDavPropertyStatus(XName name, int statusCode)
            :this (name, statusCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavPropertyStatus"/> class.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="statusCode">The status code of the operation.</param>
        /// <param name="description">The description of the operation.</param>
        public WebDavPropertyStatus(XName name, int statusCode, string description)
        {
            Guard.NotNullOrEmpty((name ?? "").ToString(), "name");

            Name = name;
            StatusCode = statusCode;
            Description = description;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public XName Name { get; private set; }

        /// <summary>
        /// Gets the status code of the operation.
        /// </summary>
        public int StatusCode { get;  private set; }

        /// <summary>
        /// Gets the description of the operation.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation on the property was successful.
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation was successfull; otherwise, <c>false</c>.
        /// </value>
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
