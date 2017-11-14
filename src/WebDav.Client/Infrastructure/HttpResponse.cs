using System;
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
            LastModified = content?.Headers?.LastModified;
        }

        public HttpContent Content { get; }

        public string ETag { get; internal set; }

        public DateTimeOffset? LastModified { get; }

        public int StatusCode { get; }

        public string Description { get; }

        public bool IsSuccessful => StatusCode >= 200 && StatusCode <= 299;
    }
}
