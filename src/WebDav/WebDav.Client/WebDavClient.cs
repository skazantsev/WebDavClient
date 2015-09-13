using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return Propfind(requestUri, new string[] { }, ApplyTo.Propfind.CollectionAndChildren, CancellationToken.None);
        }

        public Task<PropfindResponse> Propfind(string requestUri, IReadOnlyCollection<string> customProperties)
        {
            return Propfind(requestUri, customProperties, ApplyTo.Propfind.CollectionAndChildren, CancellationToken.None);
        }

        public Task<PropfindResponse> Propfind(string requestUri, ApplyTo.Propfind applyTo)
        {
            return Propfind(requestUri, new string[] { }, applyTo, CancellationToken.None);
        }

        public Task<PropfindResponse> Propfind(
            string requestUri,
            IReadOnlyCollection<string> customProperties,
            ApplyTo.Propfind applyTo)
        {
            return Propfind(requestUri, customProperties, applyTo, CancellationToken.None);
        }

        public Task<PropfindResponse> Propfind(
            string requestUri,
            IReadOnlyCollection<string> customProperties,
            CancellationToken cancellationToken)
        {
            return Propfind(requestUri, customProperties, ApplyTo.Propfind.CollectionAndChildren, cancellationToken);
        }

        public Task<PropfindResponse> Propfind(
            string requestUri,
            ApplyTo.Propfind applyTo,
            CancellationToken cancellationToken)
        {
            return Propfind(requestUri, new string[] { }, applyTo, cancellationToken);
        }

        public async Task<PropfindResponse> Propfind(string requestUri,
            IReadOnlyCollection<string> customProperties,
            ApplyTo.Propfind applyTo,
            CancellationToken cancellationToken)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(customProperties, "customProperties");

            using (var request = new HttpRequestMessage(WebDavMethod.Propfind, requestUri))
            {
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForPropfind(applyTo));
                request.Content = new StringContent(PropfindRequestBuilder.BuildRequestBody(customProperties));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if ((int)response.StatusCode != 207)
                        throw new WebDavException((int)response.StatusCode, "Wrong PROPFIND response. Multi-Status code is expected.");

                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return PropfindResponseParser.Parse(responseContent);
                }
            }
        }

        public Task Proppatch(string requestUri, IDictionary<string, string> propertiesToSet)
        {
            return Proppatch(requestUri, propertiesToSet, new string[] { }, CancellationToken.None);
        }

        public Task Proppatch(string requestUri, IReadOnlyCollection<string> propertiesToRemove)
        {
            return Proppatch(requestUri, new Dictionary<string, string>(), propertiesToRemove, CancellationToken.None);
        }

        public Task Proppatch(
            string requestUri,
            IDictionary<string, string> propertiesToSet,
            IReadOnlyCollection<string> propertiesToRemove)
        {
            return Proppatch(requestUri, propertiesToSet, propertiesToRemove, CancellationToken.None);
        }

        public Task Proppatch(
            string requestUri,
            IDictionary<string, string> propertiesToSet,
            CancellationToken cancellationToken)
        {
            return Proppatch(requestUri, propertiesToSet, new string[] {}, cancellationToken);
        }

        public Task Proppatch(
            string requestUri,
            IReadOnlyCollection<string> propertiesToRemove,
            CancellationToken cancellationToken)
        {
            return Proppatch(requestUri, new Dictionary<string, string>(), propertiesToRemove, cancellationToken);
        }

        public async Task Proppatch(
            string requestUri,
            IDictionary<string, string> propertiesToSet,
            IReadOnlyCollection<string> propertiesToRemove,
            CancellationToken cancellationToken)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");
            Guard.NotNull(propertiesToSet, "propertiesToSet");
            Guard.NotNull(propertiesToRemove, "propertiesToRemove");

            using (var request = new HttpRequestMessage(WebDavMethod.Proppatch, requestUri))
            {
                request.Content = new StringContent(ProppatchRequestBuilder.BuildRequestBody(propertiesToSet, propertiesToRemove));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if ((int) response.StatusCode != 207)
                        throw new WebDavException((int) response.StatusCode, "Wrong PROPPATCH response. Multi-Status code is expected.");
                }
            }
        }

        public Task Mkcol(string requestUri)
        {
            return Mkcol(requestUri, null, CancellationToken.None);
        }

        public Task Mkcol(string requestUri, string lockToken)
        {
            return Mkcol(requestUri, lockToken, CancellationToken.None);
        }

        public async Task Mkcol(string requestUri, string lockToken, CancellationToken cancellationToken)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Mkcol, requestUri))
            {
                if (!string.IsNullOrEmpty(lockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(lockToken));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                        throw new WebDavException((int) response.StatusCode, "Failed to create a collection.");
                }
            }
        }

        public Task<Stream> GetRawFile(string requestUri)
        {
            return GetFile(requestUri, false, CancellationToken.None);
        }

        public Task<Stream> GetRawFile(string requestUri, CancellationToken cancellationToken)
        {
            return GetFile(requestUri, false, cancellationToken);
        }

        public Task<Stream> GetProcessedFile(string requestUri)
        {
            return GetFile(requestUri, true, CancellationToken.None);
        }

        public Task<Stream> GetProcessedFile(string requestUri, CancellationToken cancellationToken)
        {
            return GetFile(requestUri, true, cancellationToken);
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
            return Delete(requestUri, null, CancellationToken.None);
        }

        public Task Delete(string requestUri, string lockToken)
        {
            return Delete(requestUri, lockToken, CancellationToken.None);
        }

        public async Task Delete(string requestUri, string lockToken, CancellationToken cancellationToken)
        {
            Guard.NotNullOrEmpty(requestUri, "requestUri");

            using (var request = new HttpRequestMessage(HttpMethod.Delete, requestUri))
            {
                if (!string.IsNullOrEmpty(lockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(lockToken));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.OK &&
                        response.StatusCode != HttpStatusCode.NoContent)
                        throw new WebDavException((int) response.StatusCode, "Failed to delete a resource.");
                }
            }
        }

        public Task PutFile(string requestUri, Stream stream, string contentType)
        {
            return PutFile(requestUri, stream, contentType, null, CancellationToken.None);
        }

        public Task PutFile(string requestUri, Stream stream, string contentType, string lockToken)
        {
            return PutFile(requestUri, stream, contentType, lockToken, CancellationToken.None);
        }

        public async Task PutFile(string requestUri, Stream stream, string contentType, string lockToken, CancellationToken cancellationToken)
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
            return Copy(sourceUri, destUri, ApplyTo.Copy.CollectionAndAncestors, null, CancellationToken.None, overwrite);
        }

        public Task Copy(string sourceUri, string destUri, string destLockToken, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, ApplyTo.Copy.CollectionAndAncestors, destLockToken, CancellationToken.None, overwrite);
        }

        public Task Copy(string sourceUri, string destUri, ApplyTo.Copy applyTo, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, applyTo, null, CancellationToken.None, overwrite);
        }

        public Task Copy(string sourceUri, string destUri, CancellationToken cancellationToken, bool overwrite = true)
        {
            return Copy(sourceUri, destUri, ApplyTo.Copy.CollectionAndAncestors, null, cancellationToken, overwrite);
        }

        public async Task Copy(string sourceUri, string destUri, ApplyTo.Copy applyTo, string destLockToken, CancellationToken cancellationToken, bool overwrite = true)
        {
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Copy, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Depth", DepthHeaderHelper.GetValueForCopy(applyTo));
                request.Headers.Add("Overwrite", overwrite ? "T" : "F");
                if (!string.IsNullOrEmpty(destLockToken))
                    request.Headers.Add("If", IfHeaderHelper.GetHeaderValue(destLockToken));
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
            return Move(sourceUri, destUri, null, null, CancellationToken.None, overwrite);
        }

        public Task Move(string sourceUri, string destUri, string sourceLockToken, string destLockToken, bool overwrite = true)
        {
            return Move(sourceUri, destUri, sourceLockToken, destLockToken, CancellationToken.None, overwrite);
        }

        public async Task Move(string sourceUri, string destUri, string sourceLockToken, string destLockToken, CancellationToken cancellationToken, bool overwrite = true)
        {
            Guard.NotNullOrEmpty(sourceUri, "sourceUri");
            Guard.NotNullOrEmpty(destUri, "destUri");

            using (var request = new HttpRequestMessage(WebDavMethod.Move, sourceUri))
            {
                request.Headers.Add("Destination", destUri);
                request.Headers.Add("Overwrite", overwrite ? "T" : "F");

                var lockTokens = new List<string>();
                if (!string.IsNullOrEmpty(sourceLockToken))
                    lockTokens.Add(IfHeaderHelper.GetHeaderValue(sourceLockToken));
                if (!string.IsNullOrEmpty(destLockToken))
                    lockTokens.Add(IfHeaderHelper.GetHeaderValue(destLockToken));
                if (lockTokens.Any())
                    request.Headers.Add("If", lockTokens);

                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
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

        public Task<List<ActiveLock>> Lock(string requestUri, LockParameters lockParams)
        {
            return Lock(requestUri, lockParams, CancellationToken.None);
        }

        public async Task<List<ActiveLock>> Lock(string requestUri, LockParameters lockParams, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Lock, requestUri))
            {
                if (lockParams.ApplyTo.HasValue)
                    request.Headers.Add("Depth", DepthHeaderHelper.GetValueForLock(lockParams.ApplyTo.Value));
                if (lockParams.Timeout.HasValue)
                    request.Headers.Add("Timeout", string.Format("Second-{0}", lockParams.Timeout.Value.TotalSeconds));
                request.Content = new StringContent(LockRequestBuilder.BuildRequestBody(lockParams));
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
            return Unlock(requestUri, lockToken, CancellationToken.None);
        }

        public async Task Unlock(string requestUri, string lockToken, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(WebDavMethod.Unlock, requestUri))
            {
                request.Headers.Add("Lock-Token", string.Format("<{0}>", lockToken));
                using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
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
