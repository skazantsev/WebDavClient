using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebDav.Infrastructure
{
    internal interface IWebDavDispatcher
    {
        Uri BaseAddress { get; }

        Task<HttpResponse> Send(Uri requestUri, HttpMethod method, RequestParameters requestParams, CancellationToken cancellationToken);
    }
}
