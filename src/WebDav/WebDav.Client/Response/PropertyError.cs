namespace WebDav
{
    public class PropertyError
    {
        public int? StatusCode { get; set; }

        public string ResponseDescription { get; set; }

        public override string ToString()
        {
            return string.Format("{{ StatusCode: {0}, ResponseDescription: {1} }}", StatusCode, ResponseDescription);
        }
    }
}
