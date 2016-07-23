using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebDav.Infrastructure;
using WebDav.Request;
using WebDav.Response;
using RequestHeaders = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>;

namespace WebDav
{
    /// <summary>
    /// Represents a WebDAV client that can perform WebDAV operations.
    /// </summary>
    public class WebDavClient : IDisposable
    {
        private IWebDavDispatcher _dispatcher;

        private IResponseParser<PropfindResponse> _propfindResponseParser;

        private IResponseParser<ProppatchResponse> _proppatchResponseParser;

        private IResponseParser<LockResponse> _lockResponseParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavClient"/> class.
        /// </summary>
        public WebDavClient()
            : this(new WebDavClientParams())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavClient"/> class.
        /// </summary>
        /// <param name="params">The parameters of the WebDAV client.</param>
        public WebDavClient(WebDavClientParams @params)
        {
            SetWebDavDispatcher(new WebDavDispatcher(ConfigureHttpClient(@params)));

            var lockResponseParser = new LockResponseParser();
            SetPropfindResponseParser(new PropfindResponseParser(lockResponseParser));
            SetProppatchResponseParser(new ProppatchResponseParser());
            SetLockResponseParser(lockResponseParser);
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="PropfindResponse" /></returns>
        public Task<PropfindResponse> Propfind(string requestUri)
        {
            return Propfind(CreateUri(requestUri), new PropfindParameters());
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="PropfindResponse" /></returns>
        public Task<PropfindResponse> Propfind(Uri requestUri)
        {
            return Propfind(requestUri, new PropfindParameters());
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the PROPFIND operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" /></returns>
        public Task<PropfindResponse> Propfind(string requestUri, PropfindParameters parameters)
        {
            return Propfind(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the PROPFIND operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" /></returns>
        public async Task<PropfindResponse> Propfind(Uri requestUri, PropfindParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var applyTo = parameters.ApplyTo ?? ApplyTo.Propfind.ResourceAndChildren;
            var headers = new RequestHeaders
            {
                new KeyValuePair<string, string>("Depth", DepthHeaderHelper.GetValueForPropfind(applyTo))
            };
            var requestBody = PropfindRequestBuilder.BuildRequestBody(parameters.CustomProperties, parameters.Namespaces);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Propfind, requestParams, parameters.CancellationToken);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _propfindResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        /// <summary>
        /// Sets and/or removes properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the PROPPATCH operation.</param>
        /// <returns>An instance of <see cref="ProppatchResponse" /></returns>
        public Task<ProppatchResponse> Proppatch(string requestUri, ProppatchParameters parameters)
        {
            return Proppatch(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Sets and/or removes properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the PROPPATCH operation.</param>
        /// <returns>An instance of <see cref="ProppatchResponse" /></returns>
        public async Task<ProppatchResponse> Proppatch(Uri requestUri, ProppatchParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken)));
            var requestBody = ProppatchRequestBuilder.BuildRequestBody(
                    parameters.PropertiesToSet,
                    parameters.PropertiesToRemove,
                    parameters.Namespaces);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Proppatch, requestParams, parameters.CancellationToken);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _proppatchResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Mkcol(string requestUri)
        {
            return Mkcol(CreateUri(requestUri), new MkColParameters());
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Mkcol(Uri requestUri)
        {
            return Mkcol(requestUri, new MkColParameters());
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the MKCOL operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Mkcol(string requestUri, MkColParameters parameters)
        {
            return Mkcol(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the MKCOL operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> Mkcol(Uri requestUri, MkColParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken)));
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Mkcol, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetRawFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), false, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri)
        {
            return GetFile(requestUri, false, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetRawFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), false, parameters.CancellationToken);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, false, parameters.CancellationToken);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), true, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri)
        {
            return GetFile(requestUri, true, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), true, parameters.CancellationToken);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" /></returns>
        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, true, parameters.CancellationToken);
        }

        internal async virtual Task<WebDavStreamResponse> GetFile(Uri requestUri, bool translate, CancellationToken cancellationToken)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders
            {
                new KeyValuePair<string, string>("Translate", translate ? "t" : "f")
            };
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Get, requestParams, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return new WebDavStreamResponse(response.StatusCode, response.Description, stream);
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Delete(string requestUri)
        {
            return Delete(CreateUri(requestUri), new DeleteParameters());
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Delete(Uri requestUri)
        {
            return Delete(requestUri, new DeleteParameters());
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the DELETE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Delete(string requestUri, DeleteParameters parameters)
        {
            return Delete(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the DELETE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> Delete(Uri requestUri, DeleteParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken)));
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Delete, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream)
        {
            return PutFile(CreateUri(requestUri), stream, new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream)
        {
            return PutFile(requestUri, stream, new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="contentType">The content type of the request body.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, string contentType)
        {
            return PutFile(CreateUri(requestUri), stream, new PutFileParameters { ContentType = contentType });
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="contentType">The content type of the request body.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, string contentType)
        {
            return PutFile(requestUri, stream, new PutFileParameters { ContentType = contentType });
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, PutFileParameters parameters)
        {
            return PutFile(CreateUri(requestUri), stream, parameters);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, PutFileParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");
            Guard.NotNull(stream, "stream");

            var headers = new RequestHeaders();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.LockToken)));
            var requestParams = new RequestParameters { Headers = headers, Content = new StreamContent(stream), ContentType = parameters.ContentType };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Put, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">A string that represents the destination <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Copy(string sourceUri, string destUri)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), new CopyParameters());
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Copy(Uri sourceUri, Uri destUri)
        {
            return Copy(sourceUri, destUri, new CopyParameters());
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">A string that represents the destination <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the COPY operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Copy(string sourceUri, string destUri, CopyParameters parameters)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the COPY operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> Copy(Uri sourceUri, Uri destUri, CopyParameters parameters)
        {
            Guard.NotNull(sourceUri, "sourceUri");
            Guard.NotNull(destUri, "destUri");

            var applyTo = parameters.ApplyTo ?? ApplyTo.Copy.ResourceAndAncestors;
            var headers = new RequestHeaders
            {
                new KeyValuePair<string, string>("Destination", GetAbsoluteUri(destUri).ToString()),
                new KeyValuePair<string, string>("Depth", DepthHeaderHelper.GetValueForCopy(applyTo)),
                new KeyValuePair<string, string>("Overwrite", parameters.Overwrite ? "T" : "F")
            };
            if (!string.IsNullOrEmpty(parameters.DestLockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.DestLockToken)));

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(sourceUri, WebDavMethod.Copy, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">A string that represents the destination <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Move(string sourceUri, string destUri)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), new MoveParameters());
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Move(Uri sourceUri, Uri destUri)
        {
            return Move(sourceUri, destUri, new MoveParameters());
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">A string that represents the destination <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the MOVE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Move(string sourceUri, string destUri, MoveParameters parameters)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="T:System.Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the MOVE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> Move(Uri sourceUri, Uri destUri, MoveParameters parameters)
        {
            Guard.NotNull(sourceUri, "sourceUri");
            Guard.NotNull(destUri, "destUri");

            var headers = new RequestHeaders
            {
                new KeyValuePair<string, string>("Destination", GetAbsoluteUri(destUri).ToString()),
                new KeyValuePair<string, string>("Overwrite", parameters.Overwrite ? "T" : "F")
            };
            if (!string.IsNullOrEmpty(parameters.SourceLockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.SourceLockToken)));
            if (!string.IsNullOrEmpty(parameters.DestLockToken))
                headers.Add(new KeyValuePair<string, string>("If", IfHeaderHelper.GetHeaderValue(parameters.DestLockToken)));

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(sourceUri, WebDavMethod.Move, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Takes out a shared lock or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <returns>An instance of <see cref="LockResponse" /></returns>
        public Task<LockResponse> Lock(string requestUri)
        {
            return Lock(CreateUri(requestUri), new LockParameters());
        }

        /// <summary>
        /// Takes out a shared lock or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <returns>An instance of <see cref="LockResponse" /></returns>
        public Task<LockResponse> Lock(Uri requestUri)
        {
            return Lock(requestUri, new LockParameters());
        }

        /// <summary>
        /// Takes out a lock of any type or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the LOCK operation.</param>
        /// <returns>An instance of <see cref="LockResponse" /></returns>
        public Task<LockResponse> Lock(string requestUri, LockParameters parameters)
        {
            return Lock(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Takes out a lock of any type or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the LOCK operation.</param>
        /// <returns>An instance of <see cref="LockResponse" /></returns>
        public async Task<LockResponse> Lock(Uri requestUri, LockParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders();
            if (parameters.ApplyTo.HasValue)
                headers.Add(new KeyValuePair<string, string>("Depth", DepthHeaderHelper.GetValueForLock(parameters.ApplyTo.Value)));
            if (parameters.Timeout.HasValue)
                headers.Add(new KeyValuePair<string, string>("Timeout", string.Format("Second-{0}", parameters.Timeout.Value.TotalSeconds)));

            var requestBody = LockRequestBuilder.BuildRequestBody(parameters);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Lock, requestParams, parameters.CancellationToken);
            if (!response.IsSuccessful)
                return new LockResponse(response.StatusCode, response.Description);

            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _lockResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="lockToken">The resource lock token.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Unlock(string requestUri, string lockToken)
        {
            return Unlock(CreateUri(requestUri), new UnlockParameters(lockToken));
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="lockToken">The resource lock token.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Unlock(Uri requestUri, string lockToken)
        {
            return Unlock(requestUri, new UnlockParameters(lockToken));
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request <see cref="T:System.Uri"/>.</param>
        /// <param name="parameters">Parameters of the UNLOCK operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Unlock(string requestUri, UnlockParameters parameters)
        {
            return Unlock(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="System.Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the UNLOCK operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public async Task<WebDavResponse> Unlock(Uri requestUri, UnlockParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new RequestHeaders
            {
                new KeyValuePair<string, string>("Lock-Token", string.Format("<{0}>", parameters.LockToken))
            };

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Unlock, requestParams, parameters.CancellationToken);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        /// <summary>
        /// Sets the dispatcher of WebDAV requests.
        /// </summary>
        /// <param name="dispatcher">The dispatcher of WebDAV http requests.</param>
        /// <returns>This instance of <see cref="WebDavClient" /> to support chain calls.</returns>
        internal WebDavClient SetWebDavDispatcher(IWebDavDispatcher dispatcher)
        {
            Guard.NotNull(dispatcher, "dispather");
            _dispatcher = dispatcher;
            return this;
        }

        /// <summary>
        /// Sets the parser of PROPFIND responses.
        /// </summary>
        /// <param name="responseParser">The parser of WebDAV PROPFIND responses.</param>
        /// <returns>This instance of <see cref="WebDavClient" /> to support chain calls.</returns>
        internal WebDavClient SetPropfindResponseParser(IResponseParser<PropfindResponse> responseParser)
        {
            Guard.NotNull(responseParser, "responseParser");
            _propfindResponseParser = responseParser;
            return this;
        }

        /// <summary>
        /// Sets the parser of PROPPATCH responses.
        /// </summary>
        /// <param name="responseParser">The parser of WebDAV PROPPATCH responses.</param>
        /// <returns>This instance of <see cref="WebDavClient" /> to support chain calls.</returns>
        internal WebDavClient SetProppatchResponseParser(IResponseParser<ProppatchResponse> responseParser)
        {
            Guard.NotNull(responseParser, "responseParser");
            _proppatchResponseParser = responseParser;
            return this;
        }

        /// <summary>
        /// Sets the parser of LOCK responses.
        /// </summary>
        /// <param name="responseParser">The parser of WebDAV LOCK responses.</param>
        /// <returns>This instance of <see cref="WebDavClient" /> to support chain calls.</returns>
        internal WebDavClient SetLockResponseParser(IResponseParser<LockResponse> responseParser)
        {
            Guard.NotNull(responseParser, "responseParser");
            _lockResponseParser = responseParser;
            return this;
        }

        private static HttpClient ConfigureHttpClient(WebDavClientParams @params)
        {
            var httpHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                PreAuthenticate = @params.PreAuthenticate,
                UseDefaultCredentials = @params.UseDefaultCredentials,
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

            var httpClient = new HttpClient(httpHandler, true)
            {
                BaseAddress = @params.BaseAddress,
                Timeout = @params.Timeout
            };

            foreach (var header in @params.DefaultRequestHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return httpClient;
        }

        private static Uri CreateUri(string requestUri)
        {
            return !string.IsNullOrEmpty(requestUri) ? new Uri(requestUri, UriKind.RelativeOrAbsolute) : null;
        }

        private static Exception CreateInvalidUriException()
        {
            return
                new InvalidOperationException(
                    "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        private static Encoding GetResponseEncoding(HttpContent content, Encoding fallbackEncoding)
        {
            if (content.Headers.ContentType == null || content.Headers.ContentType.CharSet == null)
                return fallbackEncoding;

            try
            {
                return Encoding.GetEncoding(content.Headers.ContentType.CharSet);
            }
            catch (ArgumentException)
            {
                return fallbackEncoding;
            }
        }

        private static async Task<string> ReadContentAsString(HttpContent content)
        {
            var data = await content.ReadAsByteArrayAsync();
            return GetResponseEncoding(content, Encoding.UTF8).GetString(data);
        }

        private Uri GetAbsoluteUri(Uri uri)
        {
            if (uri == null && _dispatcher.BaseAddress == null)
                throw CreateInvalidUriException();

            if (uri == null)
                return _dispatcher.BaseAddress;

            if (uri.IsAbsoluteUri)
                return uri;

            if (_dispatcher.BaseAddress == null)
                throw CreateInvalidUriException();
            return new Uri(_dispatcher.BaseAddress, uri);
        }

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed/unmanaged resources.
        /// Disposes the underlying HttpClient.
        /// </summary>
        public void Dispose()
        {
            DisposeManagedResources();
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            var disposableDispatcher = _dispatcher as IDisposable;
            if (disposableDispatcher != null)
            {
                disposableDispatcher.Dispose();
            }
        }

        #endregion
    }
}
