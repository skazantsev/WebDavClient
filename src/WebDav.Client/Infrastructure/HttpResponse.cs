using System.Net.Http;

namespace WebDav.Infrastructure
{
    internal class HttpResponse
    {
        public HttpResponse(HttpContent content, int statusCode)
            : this (content, statusCode, string.Empty)
        {
        }

        public HttpResponse(HttpContent content, int statusCode, string description)
        {
            Content = content;
            StatusCode = statusCode;
            Description = description;
        }

        public HttpContent Content { get; private set; }

        public int StatusCode { get; private set; }

        public string Description { get; private set; }

        public bool IsSuccessful
        {
            get { return StatusCode >= 200 && StatusCode <= 299; }
        }
    }
}
