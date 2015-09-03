using System;
using System.Collections.Generic;
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
            return Propfind(requestUri, new string[] { }, ApplyTo.Propfind.CollectionAndChildren);
        }

        public Task<PropfindResponse> Propfind(string requestUri, IReadOnlyCollection<string> customProperties)
        {
            return Propfind(requestUri, customProperties, ApplyTo.Propfind.CollectionAndChildren);
        }

        public Task<PropfindResponse> Propfind(string requestUri, ApplyTo.Propfind applyTo)
        {
            return Propfind(requestUri, new string[] { }, applyTo);
        }

        public async Task<PropfindResponse> Propfind(string requestUri, IReadOnlyCollection<string> customProperties, ApplyTo.Propfind applyTo)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(customProperties, "customProperties");

            using (var request = new HttpRequestMessage(WebDavMethod.Propfind, requestUri))
            {
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForPropfind(applyTo));
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

        public Task Proppatch(string requestUri, IDictionary<string, string> propertiesToSet)
        {
            return Proppatch(requestUri, propertiesToSet, new string[] { });
        }

        public Task Proppatch(string requestUri, IReadOnlyCollection<string> propertiesToRemove)
        {
            return Proppatch(requestUri, new Dictionary<string, string>(), propertiesToRemove);
        }

        public async Task Proppatch(string requestUri, IDictionary<string, string> propertiesToSet, IReadOnlyCollection<string> propertiesToRemove)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(propertiesToSet, "propertiesToSet");
            Guard.NotNull(propertiesToRemove, "propertiesToRemove");

            using (var request = new HttpRequestMessage(WebDavMethod.Proppatch, requestUri))
            {
                request.Content = new StringContent(ProppatchRequestBuilder.BuildRequestBody(propertiesToSet, propertiesToRemove));
                using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
                {
                    if ((int) response.StatusCode != 207)
                        throw new WebDavException((int) response.StatusCode, "Wrong PROPPATCH response. Multi-Status code is expected.");
                }
            }
        }

        public async Task Mkcol(string requestUri)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Mkcol, requestUri))
            using (var response = await _httpClient.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode != HttpStatusCode.Created)
                    throw new WebDavException((int)response.StatusCode, "Failed to create a collection.");
            }
        }

        public Task<Stream> GetRawFile(string requestUri)
        {
            return GetFile(requestUri, false);
        }

        public Task<Stream> GetProcessedFile(string requestUri)
        {
            return GetFile(requestUri, true);
        }

        private async Task<Stream> GetFile(string requestUri, bool translate)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

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
            Guard.NotNullOrEmpty(requestUri, "requestUri");

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
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(stream, "stream");

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
            return Copy(sourceUri, destUri, ApplyTo.Copy.CollectionAndAncestors, CancellationToken.None, overwrite);
        }

        public Task Copy(string sourceUri, string destUri, ApplyTo.Copy applyTo, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, applyTo, CancellationToken.None, overwrite);
        }

        public Task Copy(string sourceUri, string destUri, CancellationToken cancellationToken, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, ApplyTo.Copy.CollectionAndAncestors, cancellationToken, overwrite);
        }

        public async Task Copy(string sourceUri, string destUri, ApplyTo.Copy applyTo, CancellationToken cancellationToken, bool overwrite = true)
        {
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Copy, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForCopy(applyTo));
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
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

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
