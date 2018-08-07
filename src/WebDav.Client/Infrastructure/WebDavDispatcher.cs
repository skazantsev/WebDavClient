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

        public async Task<HttpResponseMessage> Send(
            Uri requestUri,
            HttpMethod method,
            RequestParameters requestParams,
            CancellationToken cancellationToken,
            HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                foreach (var header in requestParams.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                if (requestParams.Content != null)
                {
                    request.Content = requestParams.Content;
                    if (!string.IsNullOrEmpty(requestParams.ContentType))
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(requestParams.ContentType);
                }

                var response = await _httpClient.SendAsync(request, httpCompletionOption, cancellationToken).ConfigureAwait(false);
                return response;
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
