using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebDav.Client.Domain;
using WebDav.Infrastructure;
using WebDav.Request;
using WebDav.Response;

namespace WebDav
{
    public class WebDavClient : IWebDavClient
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
            Initialize(ConfigureHttpClient(@params));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavClient"/> class.
        /// </summary>
        /// <param name="httpClient">The pre-configured instance of <see cref="HttpClient"/>.</param>
        public WebDavClient(HttpClient httpClient)
        {
            Initialize(httpClient);
        }

        private void Initialize(HttpClient httpClient)
        {
            SetWebDavDispatcher(new WebDavDispatcher(httpClient));

            var lockResponseParser = new LockResponseParser();
            SetPropfindResponseParser(new PropfindResponseParser(lockResponseParser));
            SetProppatchResponseParser(new ProppatchResponseParser());
            SetLockResponseParser(lockResponseParser);
        }

        public Task<PropfindResponse> Propfind(string requestUri)
        {
            return Propfind(CreateUri(requestUri), new PropfindParameters());
        }

        public Task<PropfindResponse> Propfind(Uri requestUri)
        {
            return Propfind(requestUri, new PropfindParameters());
        }

        public Task<PropfindResponse> Propfind(string requestUri, PropfindParameters parameters)
        {
            return Propfind(CreateUri(requestUri), parameters);
        }

        public async Task<PropfindResponse> Propfind(Uri requestUri, PropfindParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var applyTo = parameters.ApplyTo ?? ApplyTo.Propfind.ResourceAndChildren;
            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForPropfind(applyTo))
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestBody = PropfindRequestBuilder.BuildRequestBody(parameters.CustomProperties, parameters.Namespaces);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Propfind, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _propfindResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        public Task<ProppatchResponse> Proppatch(string requestUri, ProppatchParameters parameters)
        {
            return Proppatch(CreateUri(requestUri), parameters);
        }

        public async Task<ProppatchResponse> Proppatch(Uri requestUri, ProppatchParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestBody = ProppatchRequestBuilder.BuildRequestBody(
                    parameters.PropertiesToSet,
                    parameters.PropertiesToRemove,
                    parameters.Namespaces);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Proppatch, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _proppatchResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        public Task<WebDavResponse> Mkcol(string requestUri)
        {
            return Mkcol(CreateUri(requestUri), new MkColParameters());
        }

        public Task<WebDavResponse> Mkcol(Uri requestUri)
        {
            return Mkcol(requestUri, new MkColParameters());
        }

        public Task<WebDavResponse> Mkcol(string requestUri, MkColParameters parameters)
        {
            return Mkcol(CreateUri(requestUri), parameters);
        }

        public async Task<WebDavResponse> Mkcol(Uri requestUri, MkColParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Mkcol, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        public Task<WebDavStreamResponse> GetRawFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), false, new GetFileParameters());
        }

        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri)
        {
            return GetFile(requestUri, false, new GetFileParameters());
        }

        public Task<WebDavStreamResponse> GetRawFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), false, parameters);
        }

        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, false, parameters);
        }

        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), true, new GetFileParameters());
        }

        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri)
        {
            return GetFile(requestUri, true, new GetFileParameters());
        }

        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), true, parameters);
        }

        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, true, parameters);
        }

        internal virtual async Task<WebDavStreamResponse> GetFile(Uri requestUri, bool translate, GetFileParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.Translate, translate ? "t" : "f")
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Get, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return new WebDavStreamResponse(response.StatusCode, response.Description, stream);
        }

        public Task<WebDavResponse> Delete(string requestUri)
        {
            return Delete(CreateUri(requestUri), new DeleteParameters());
        }

        public Task<WebDavResponse> Delete(Uri requestUri)
        {
            return Delete(requestUri, new DeleteParameters());
        }

        public Task<WebDavResponse> Delete(string requestUri, DeleteParameters parameters)
        {
            return Delete(CreateUri(requestUri), parameters);
        }

        public async Task<WebDavResponse> Delete(Uri requestUri, DeleteParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Delete, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        public Task<WebDavResponse> PutFile(string requestUri, Stream stream)
        {
            return PutFile(CreateUri(requestUri), new StreamContent(stream), new PutFileParameters());
        }

        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream)
        {
            return PutFile(requestUri, new StreamContent(stream), new PutFileParameters());
        }

        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, string contentType)
        {
            return PutFile(CreateUri(requestUri), new StreamContent(stream), new PutFileParameters { ContentType = contentType });
        }

        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, string contentType)
        {
            return PutFile(requestUri, new StreamContent(stream), new PutFileParameters { ContentType = contentType });
        }

        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, PutFileParameters parameters)
        {
            return PutFile(CreateUri(requestUri), new StreamContent(stream), parameters);
        }

        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, PutFileParameters parameters)
        {
          return PutFile(requestUri, new StreamContent(stream), parameters);
        }

        public Task<WebDavResponse> PutFile(string requestUri, HttpContent content)
        {
            return PutFile(CreateUri(requestUri), content, new PutFileParameters());
        }

        public Task<WebDavResponse> PutFile(Uri requestUri, HttpContent content)
        {
            return PutFile(requestUri, content, new PutFileParameters());
        }

        public Task<WebDavResponse> PutFile(string requestUri, HttpContent content, PutFileParameters parameters)
        {
            return PutFile(CreateUri(requestUri), content, parameters);
        }

        public async Task<WebDavResponse> PutFile(Uri requestUri, HttpContent content, PutFileParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");
            Guard.NotNull(content, "content");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers, Content = content, ContentType = parameters.ContentType };
            var response = await _dispatcher.Send(requestUri, HttpMethod.Put, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        public Task<WebDavResponse> Copy(string sourceUri, string destUri)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), new CopyParameters());
        }

        public Task<WebDavResponse> Copy(Uri sourceUri, Uri destUri)
        {
            return Copy(sourceUri, destUri, new CopyParameters());
        }

        public Task<WebDavResponse> Copy(string sourceUri, string destUri, CopyParameters parameters)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        public async Task<WebDavResponse> Copy(Uri sourceUri, Uri destUri, CopyParameters parameters)
        {
            Guard.NotNull(sourceUri, "sourceUri");
            Guard.NotNull(destUri, "destUri");

            var applyTo = parameters.ApplyTo ?? ApplyTo.Copy.ResourceAndAncestors;
            var headerBuilder = new HeaderBuilder()
                .Add(WebDavHeaders.Destination, GetAbsoluteUri(destUri).AbsoluteUri)
                .Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForCopy(applyTo))
                .Add(WebDavHeaders.Overwrite, parameters.Overwrite ? "T" : "F");

            if (!string.IsNullOrEmpty(parameters.DestLockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(sourceUri, WebDavMethod.Copy, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        public Task<WebDavResponse> Move(string sourceUri, string destUri)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), new MoveParameters());
        }

        public Task<WebDavResponse> Move(Uri sourceUri, Uri destUri)
        {
            return Move(sourceUri, destUri, new MoveParameters());
        }

        public Task<WebDavResponse> Move(string sourceUri, string destUri, MoveParameters parameters)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        public async Task<WebDavResponse> Move(Uri sourceUri, Uri destUri, MoveParameters parameters)
        {
            Guard.NotNull(sourceUri, "sourceUri");
            Guard.NotNull(destUri, "destUri");

            var headerBuilder = new HeaderBuilder()
                .Add(WebDavHeaders.Destination, GetAbsoluteUri(destUri).AbsoluteUri)
                .Add(WebDavHeaders.Overwrite, parameters.Overwrite ? "T" : "F");

            if (!string.IsNullOrEmpty(parameters.SourceLockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.SourceLockToken));
            if (!string.IsNullOrEmpty(parameters.DestLockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(sourceUri, WebDavMethod.Move, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse(response.StatusCode, response.Description);
        }

        public Task<LockResponse> Lock(string requestUri)
        {
            return Lock(CreateUri(requestUri), new LockParameters());
        }

        public Task<LockResponse> Lock(Uri requestUri)
        {
            return Lock(requestUri, new LockParameters());
        }

        public Task<LockResponse> Lock(string requestUri, LockParameters parameters)
        {
            return Lock(CreateUri(requestUri), parameters);
        }

        public async Task<LockResponse> Lock(Uri requestUri, LockParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (parameters.ApplyTo.HasValue)
                headerBuilder.Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForLock(parameters.ApplyTo.Value));
            if (parameters.Timeout.HasValue)
                headerBuilder.Add(WebDavHeaders.Timeout, $"Second-{parameters.Timeout.Value.TotalSeconds}");

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestBody = LockRequestBuilder.BuildRequestBody(parameters);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody) };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Lock, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessful)
                return new LockResponse(response.StatusCode, response.Description);

            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _lockResponseParser.Parse(responseContent, response.StatusCode, response.Description);
        }

        public Task<WebDavResponse> Unlock(string requestUri, string lockToken)
        {
            return Unlock(CreateUri(requestUri), new UnlockParameters(lockToken));
        }

        public Task<WebDavResponse> Unlock(Uri requestUri, string lockToken)
        {
            return Unlock(requestUri, new UnlockParameters(lockToken));
        }

        public Task<WebDavResponse> Unlock(string requestUri, UnlockParameters parameters)
        {
            return Unlock(CreateUri(requestUri), parameters);
        }

        public async Task<WebDavResponse> Unlock(Uri requestUri, UnlockParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.LockToken, $"<{parameters.LockToken}>")
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher.Send(requestUri, WebDavMethod.Unlock, requestParams, parameters.CancellationToken).ConfigureAwait(false);
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
                httpHandler.UseDefaultCredentials = false;
            }
            if (@params.Proxy != null)
            {
                httpHandler.Proxy = @params.Proxy;
            }

            var httpClient = new HttpClient(httpHandler, true)
            {
                BaseAddress = @params.BaseAddress
            };

            if (@params.Timeout.HasValue)
            {
                httpClient.Timeout = @params.Timeout.Value;
            }

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
            if (content.Headers.ContentType?.CharSet == null)
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
            var data = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return GetResponseEncoding(content, Encoding.UTF8).GetString(data, 0, data.Length);
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
            disposableDispatcher?.Dispose();
        }

        #endregion
    }
}
