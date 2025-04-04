﻿using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebDav
{
    internal class RequestParameters
    {
        public RequestParameters()
        {
            Headers = new List<KeyValuePair<string, string>>();
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> Headers { get; set; }

        public HttpContent? Content { get; set; }

        public MediaTypeHeaderValue? ContentType { get; set; }
    }
}
