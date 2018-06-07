using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebDav.Infrastructure
{
    internal class WebDavDispatcher : IWebDavDispatcher, IDisposable
    {
        private readonly HttpClient _httpClient;

        public WebDavDispatcher(HttpClient httpClient)
        {
            Guard.NotNull(httpClient, "httpClient");

            _httpClient = httpClient;
        }

        public Uri BaseAddress => _httpClient.BaseAddress;

        public async Task<HttpResponse> Send(Uri requestUri, HttpMethod method, RequestParameters requestParams, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                if (requestParams.Content != null)
                {
                    request.Content = requestParams.Content;
                    if (!string.IsNullOrEmpty(requestParams.ContentType))
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(requestParams.ContentType);
                }

                foreach (var header in requestParams.Headers)
                {
                    // When setting Content-Range as a Header on the HTTPRequestHeader directly, the call to SendAsync crashes out with the following exception:
                    // "Misused header name. Make sure request headers are used with HttpRequestMessage, response headers with HttpResponseMessage, and content headers with HttpContent objects."
                    if (header.Key.Equals("Content-Range"))
                    {
                        request.Content.Headers.ContentRange = ContentRangeHeaderValue.Parse(header.Value);
                    }
                    else
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                return new HttpResponse(response.Content, (int)response.StatusCode, response.ReasonPhrase);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            DisposeManagedResources();
        }

        protected virtual void DisposeManagedResources()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }
}
