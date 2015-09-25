namespace WebDav
{
    public class WebDavResponse
    {
        public WebDavResponse(int statusCode)
        {
            StatusCode = statusCode;
        }

        public WebDavResponse(int statusCode, string description)
        {
            StatusCode = statusCode;
            Description = description;
        }

        public int StatusCode { get; private set; }

        public string Description { get; private set; }

        public virtual bool IsSuccessful
        {
            get { return StatusCode >= 200 && StatusCode <= 299; }
        }

        public override string ToString()
        {
            return string.Format("WebDav response: StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
