namespace WebDav
{
    public class PropertyError
    {
        public PropertyError(int statusCode)
        {
            StatusCode = statusCode;
        }

        public PropertyError(int statusCode, string description)
        {
            StatusCode = statusCode;
            Description = description;
        }

        public int StatusCode { get; private set; }

        public string Description { get; private set; }

        public override string ToString()
        {
            return string.Format("Property error: StatusCode: {0}, Description: {1}", StatusCode, Description);
        }
    }
}
