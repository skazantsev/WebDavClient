using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
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
            return Propfind(requestUri, new PropfindParameters());
        }

        public async Task<PropfindResponse> Propfind(string requestUri, PropfindParameters parameters)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Propfind, requestUri))
            {
                var applyTo = parameters.ApplyTo ?? ApplyTo.Propfind.ResourceAndChildren;
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForPropfind(applyTo));
                var requestBody = PropfindRequestBuilder.BuildRequestBody(parameters.CustomProperties, parameters.Namespaces);
                request.Content = new StringContent(requestBody);
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if ((int)response.StatusCode != 207)
                        throw new WebDavException((int)response.StatusCode, "Wrong PROPFIND response. Multi-Status code is expected.");

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return PropfindResponseParser.Parse(responseContent);
                }
            }
        }

        public async Task Proppatch(string requestUri, ProppatchParameters parameters)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Proppatch, requestUri))
            {
                var requestBody = ProppatchRequestBuilder.BuildRequestBody(
                    parameters.PropertiesToSet,
                    parameters.PropertiesToRemove,
                    parameters.Namespaces);
                request.Content = new StringContent(requestBody);
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if ((int)response.StatusCode != 207)
                        throw new WebDavException((int)response.StatusCode, "Wrong PROPPATCH response. Multi-Status code is expected.");
                }
            }
        }

        public Task Mkcol(string requestUri)
        {
            return Mkcol(requestUri, new MkColParameters());
        }

        public async Task Mkcol(string requestUri, MkColParameters parameters)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Mkcol, requestUri))
            {
                if (!string.IsNullOrEmpty(parameters.LockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken));
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                        throw new WebDavException((int)response.StatusCode, "Failed to create a collection.");
                }
            }
        }

        public Task<Stream> GetRawFile(string requestUri)
        {
            return GetFile(requestUri, false, CancellationToken.None);
        }

        public Task<Stream> GetRawFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, false, parameters.CancellationToken);
        }

        public Task<Stream> GetProcessedFile(string requestUri)
        {
            return GetFile(requestUri, true, CancellationToken.None);
        }

        public Task<Stream> GetProcessedFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, true, parameters.CancellationToken);
        }

        private async Task<Stream> GetFile(string requestUri, bool translate, CancellationToken cancellationToken)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                request.Headers.Add("Translate", translate ? "t" : "f");
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    throw new WebDavException((int)response.StatusCode, "Failed to get a file.");

                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }

        public Task Delete(string requestUri)
        {
            return Delete(requestUri, new DeleteParameters());
        }

        public async Task Delete(string requestUri, DeleteParameters parameters)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
            {
                if (!string.IsNullOrEmpty(parameters.LockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken));
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK &&
                        response.StatusCode != HttpStatusCode.NoContent)
                        throw new WebDavException((int)response.StatusCode, "Failed to delete a resource.");
                }
            }
        }

        public Task PutFile(string requestUri, Stream stream)
        {
            return PutFile(requestUri, stream, new PutFileParameters());
        }

        public Task PutFile(string requestUri, Stream stream, string contentType)
        {
            return PutFile(requestUri, stream, new PutFileParameters { ContentType = contentType });
        }

        public async Task PutFile(string requestUri, Stream stream, PutFileParameters parameters)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(stream, "stream");

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(parameters.ContentType);
            using (var response = await _httpClient.PutAsync(requestUri, fileContent, parameters.CancellationToken).ConfigureAwait(false))
            {
                if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.NoContent)
                    throw new WebDavException((int)response.StatusCode, "Failed to upload a file.");
            }
        }

        public Task Copy(string sourceUri, string destUri)
        {
            return Copy(sourceUri, destUri, new CopyParameters());
        }

        public async Task Copy(string sourceUri, string destUri, CopyParameters parameters)
        {
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Copy, sourceUri))
            {
                var applyTo = parameters.ApplyTo ?? ApplyTo.Copy.ResourceAndAncestors;
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForCopy(applyTo));
                request.Headers.Add("Overwrite", parameters.Overwrite ? "T" : "F");
                if (!string.IsNullOrEmpty(parameters.DestLockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
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
            return Move(sourceUri, destUri, new MoveParameters());
        }

        public async Task Move(string sourceUri, string destUri, MoveParameters parameters)
        {
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Move, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Overwrite", parameters.Overwrite ? "T" : "F");

                var lockTokens = new List<string>();
                if (!string.IsNullOrEmpty(parameters.SourceLockToken))
                    lockTokens.Add(IfHeaderHelper.GetHeaderValue(parameters.SourceLockToken));
                if (!string.IsNullOrEmpty(parameters.DestLockToken))
                    lockTokens.Add(IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));
                if (lockTokens.Any())
                    request.Headers.Add("If", lockTokens);

                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK &&
                    response.StatusCode != HttpStatusCode.Created &&
                    response.StatusCode != HttpStatusCode.NoContent)
                        throw new WebDavException((int)response.StatusCode, "Failed to move a resource.");
                }
            }
        }

        public Task<List<ActiveLock>> Lock(string requestUri)
        {
            return Lock(requestUri, new LockParameters(), CancellationToken.None);
        }

        public Task<List<ActiveLock>> Lock(string requestUri, LockParameters parameters)
        {
            return Lock(requestUri, parameters, CancellationToken.None);
        }

        public async Task<List<ActiveLock>> Lock(string requestUri, LockParameters parameters, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Lock, requestUri))
            {
                if (parameters.ApplyTo.HasValue)
                    request.Headers.Add("Depth", DepthHeaderHelper.GetValueForLock(parameters.ApplyTo.Value));
                if (parameters.Timeout.HasValue)
                    request.Headers.Add("Timeout", string.Format("Second-{0}", parameters.Timeout.Value.TotalSeconds));
                request.Content = new StringContent(LockRequestBuilder.BuildRequestBody(parameters));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new WebDavException((int)response.StatusCode, "Failed to acquire a lock.");

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return LockResponseParser.Parse(responseContent);
                }
            }
        }

        public Task Unlock(string requestUri, string lockToken)
        {
            return Unlock(requestUri, new UnlockParameters { LockToken = lockToken });
        }

        public async Task Unlock(string requestUri, UnlockParameters parameters)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Unlock, requestUri))
            {
                request.Headers.Add("Lock-Token", string.Format("<{0}>", parameters.LockToken));
                using (var response = await _httpClient.SendAsync(request, parameters.CancellationToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new WebDavException((int)response.StatusCode, "Failed to unlock a resource.");
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
