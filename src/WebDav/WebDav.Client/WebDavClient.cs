using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebDav.Exceptions;
using WebDav.Helpers;
using WebDav.Request;
using WebDav.Response;

namespace WebDav
{
    public class WebDavClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public WebDavClient()
            : this(new WebDavClientParams())
        {
        }

        public WebDavClient(WebDavClientParams @params)
        {
            _httpClient = ConfigureHttpClient(@params);
        }

        public Task<PropfindResponse> Propfind(string requestUri)
        {
            return Propfind(requestUri, new string[] {});
        }

        public async Task<PropfindResponse> Propfind(string requestUri, params string[] customProperties)
        {
            if (customProperties == null)
                throw new ArgumentNullException("customProperties");

            using (var request = new HttpRequestMessage(WebDavMethod.Propfind, requestUri))
            {
                request.Headers.Add("Depth", "1");
                request.Content = new StringContent(PropfindRequestBuilder.BuildRequestBody(customProperties));
                using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    if ((int) response.StatusCode != 207)
                        throw new WebDavException((int) response.StatusCode, "Wrong PROPFIND response. Multi-Status code is expected.");

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return PropfindResponseParser.Parse(responseContent);
                }
            }
        }

        public async Task Mkcol(string requestUri)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Mkcol, requestUri))
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode != HttpStatusCode.Created)
                    throw new WebDavException((int)response.StatusCode, "Failed to create a collection.");
            }
        }

        public async Task<Stream> GetFile(string requestUri, bool translate = false)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                request.Headers.Add("Translate", translate ? "t" : "f");
                var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new WebDavException((int)response.StatusCode, "Failed to get a file.");

                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }

        public async Task Delete(string requestUri)
        {
            using (var response = await _httpClient.DeleteAsync(requestUri).ConfigureAwait(false))
            {
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.NoContent)
                    throw new WebDavException((int)response.StatusCode, "Failed to delete a resource.");
            }
        }

        public Task PutFile(string requestUri, Stream stream, string contentType)
        {
            return PutFile(requestUri, stream, contentType, CancellationToken.None);
        }

        public async Task PutFile(string requestUri, Stream stream, string contentType, CancellationToken cancellationToken)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            using (var response = await _httpClient.PutAsync(requestUri, fileContent, cancellationToken).ConfigureAwait(false))
            {
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.NoContent)
                    throw new WebDavException((int)response.StatusCode, "Failed to upload a file.");
            }
        }

        public Task Copy(string sourceUri, string destUri, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, CancellationToken.None, overwrite);
        }

        public async Task Copy(string sourceUri, string destUri, CancellationToken cancellationToken, bool overwrite = true)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Copy, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Overwrite", overwrite ? "T" : "F");
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.NoContent)
                        throw new WebDavException((int)response.StatusCode, "Failed to copy a resource.");
                }
            }
        }

        public Task Move(string sourceUri, string destUri, bool overwrite = true)
        {
            return Move(sourceUri, destUri, CancellationToken.None, overwrite);
        }

        public async Task Move(string sourceUri, string destUri, CancellationToken cancellationToken, bool overwrite = true)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Move, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Overwrite", overwrite ? "T" : "F");
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.NoContent)
                        throw new WebDavException((int)response.StatusCode, "Failed to move a resource.");
                }
            }
        }

        private static HttpClient ConfigureHttpClient(WebDavClientParams @params)
        {
            var httpHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                PreAuthenticate = @params.PreAuthenticate,
                UseDefaultCredentials = @params.AuthenticateAsCurrentUser,
                UseProxy = @params.UseProxy
            };
            if (@params.Credentials != null)
            {
                httpHandler.Credentials = @params.Credentials;
            }
            if (@params.Proxy != null)
            {
                httpHandler.Proxy = @params.Proxy;
            }

            var httpClient = new HttpClient(httpHandler, true);
            foreach (var header in @params.DefaultRequestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return httpClient;
        }

        #region IDisposable

        public void Dispose()
        {
            DisposeManagedResources();
        }

        protected virtual void DisposeManagedResources()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }

        #endregion
    }
}
