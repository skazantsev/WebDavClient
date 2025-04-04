﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebDav
{
    /// <summary>
    /// Represents a WebDAV client that can perform WebDAV operations.
    /// </summary>
    public class WebDavClient : IWebDavClient
    {
        private IWebDavDispatcher? _dispatcher;

        private IResponseParser<PropfindResponse>? _propfindResponseParser;

        private IResponseParser<ProppatchResponse>? _proppatchResponseParser;

        private IResponseParser<LockResponse>? _lockResponseParser;

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

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public Task<PropfindResponse> Propfind(string requestUri)
        {
            return Propfind(CreateUri(requestUri), new PropfindParameters());
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public Task<PropfindResponse> Propfind(Uri requestUri)
        {
            return Propfind(requestUri, new PropfindParameters());
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the PROPFIND operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public Task<PropfindResponse> Propfind(string requestUri, PropfindParameters parameters)
        {
            return Propfind(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Retrieves properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the PROPFIND operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public async Task<PropfindResponse> Propfind(Uri requestUri, PropfindParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var applyTo = parameters.ApplyTo ?? ApplyTo.Propfind.ResourceAndChildren;
            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.Depth, DepthHeaderHelper.GetValueForPropfind(applyTo))
                .AddWithOverwrite(parameters.Headers)
                .Build();

            HttpContent? requestBody = null;
            if (parameters.RequestType != PropfindRequestType.AllPropertiesImplied)
            {
                var content = PropfindRequestBuilder.BuildRequest(parameters.RequestType, parameters.CustomProperties, parameters.Namespaces);
                requestBody = new StringContent(content);
            }

            var requestParams = new RequestParameters { Headers = headers, Content = requestBody, ContentType = parameters.ContentType };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Propfind, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _propfindResponseParser!.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Sets and/or removes properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the PROPPATCH operation.</param>
        /// <returns>An instance of <see cref="ProppatchResponse" />.</returns>
        public Task<ProppatchResponse> Proppatch(string requestUri, ProppatchParameters parameters)
        {
            return Proppatch(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Sets and/or removes properties defined on the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the PROPPATCH operation.</param>
        /// <returns>An instance of <see cref="ProppatchResponse" />.</returns>
        public async Task<ProppatchResponse> Proppatch(Uri requestUri, ProppatchParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestBody = ProppatchRequestBuilder.BuildRequestBody(
                    parameters.PropertiesToSet,
                    parameters.PropertiesToRemove,
                    parameters.Namespaces);
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody), ContentType = parameters.ContentType };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Proppatch, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _proppatchResponseParser!.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Mkcol(string requestUri)
        {
            return Mkcol(CreateUri(requestUri), new MkColParameters());
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Mkcol(Uri requestUri)
        {
            return Mkcol(requestUri, new MkColParameters());
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the MKCOL operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Mkcol(string requestUri, MkColParameters parameters)
        {
            return Mkcol(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Creates a new collection resource at the location specified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the MKCOL operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public async Task<WebDavResponse> Mkcol(Uri requestUri, MkColParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Mkcol, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetRawFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), false, new GetFileParameters());
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri)
        {
            return GetFile(requestUri, false, new GetFileParameters());
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetRawFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), false, parameters);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return it without processing.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetRawFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, false, parameters);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri)
        {
            return GetFile(CreateUri(requestUri), true, new GetFileParameters());
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri)
        {
            return GetFile(requestUri, true, new GetFileParameters());
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetProcessedFile(string requestUri, GetFileParameters parameters)
        {
            return GetFile(CreateUri(requestUri), true, parameters);
        }

        /// <summary>
        /// Retrieves the file identified by the request URI telling the server to return a processed response, if possible.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="WebDavStreamResponse" />.</returns>
        public Task<WebDavStreamResponse> GetProcessedFile(Uri requestUri, GetFileParameters parameters)
        {
            return GetFile(requestUri, true, parameters);
        }

        internal virtual async Task<WebDavStreamResponse> GetFile(Uri requestUri, bool translate, GetFileParameters parameters)
        {
            var response = await GetFileResponse(requestUri, translate, parameters).ConfigureAwait(false);

            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return new WebDavStreamResponse(response, stream);
        }

        /// <summary>
        /// Retrieves the raw http response of a file identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="translate">A parameter indicating if the response can be processed by the web server.</param>
        /// <param name="parameters">Parameters of the GET operation.</param>
        /// <returns>An instance of <see cref="HttpResponseMessage" />.</returns>
        public async Task<HttpResponseMessage> GetFileResponse(Uri requestUri, bool translate, GetFileParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.Translate, translate ? "t" : "f")
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(
                requestUri,
                HttpMethod.Get,
                requestParams,
                parameters.CancellationToken,
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Delete(string requestUri)
        {
            return Delete(CreateUri(requestUri), new DeleteParameters());
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Delete(Uri requestUri)
        {
            return Delete(requestUri, new DeleteParameters());
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the DELETE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Delete(string requestUri, DeleteParameters parameters)
        {
            return Delete(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Deletes the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the DELETE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public async Task<WebDavResponse> Delete(Uri requestUri, DeleteParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(requestUri, HttpMethod.Delete, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream)
        {
            return PutFile(CreateUri(requestUri), new StreamContent(stream), new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream)
        {
            return PutFile(requestUri, new StreamContent(stream), new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="contentType">The content type of the request body.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, string contentType)
        {
            var @params = new PutFileParameters { ContentType = new MediaTypeHeaderValue(contentType) };
            return PutFile(CreateUri(requestUri), new StreamContent(stream), @params);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="contentType">The content type of the request body.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, string contentType)
        {
            var @params = new PutFileParameters { ContentType = new MediaTypeHeaderValue(contentType) };
            return PutFile(requestUri, new StreamContent(stream), @params);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(string requestUri, Stream stream, PutFileParameters parameters)
        {
            return PutFile(CreateUri(requestUri), new StreamContent(stream), parameters);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="stream">The stream of content of the resource.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, Stream stream, PutFileParameters parameters)
        {
            return PutFile(requestUri, new StreamContent(stream), parameters);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="content">The content to pass to the request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(string requestUri, HttpContent content)
        {
            return PutFile(CreateUri(requestUri), content, new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="content">The content to pass to the request.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(Uri requestUri, HttpContent content)
        {
            return PutFile(requestUri, content, new PutFileParameters());
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="content">The content to pass to the request.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> PutFile(string requestUri, HttpContent content, PutFileParameters parameters)
        {
            return PutFile(CreateUri(requestUri), content, parameters);
        }

        /// <summary>
        /// Requests the resource to be stored under the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="content">The content to pass to the request.</param>
        /// <param name="parameters">Parameters of the PUT operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public async Task<WebDavResponse> PutFile(Uri requestUri, HttpContent content, PutFileParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");
            Guard.NotNull(content, "content");

            var headerBuilder = new HeaderBuilder();
            if (!string.IsNullOrEmpty(parameters.LockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.LockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers, Content = content, ContentType = parameters.ContentType };
            var response = await _dispatcher!.Send(requestUri, HttpMethod.Put, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source URI.</param>
        /// <param name="destUri">A string that represents the destination URI.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Copy(string sourceUri, string destUri)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), new CopyParameters());
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Copy(Uri sourceUri, Uri destUri)
        {
            return Copy(sourceUri, destUri, new CopyParameters());
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source URI.</param>
        /// <param name="destUri">A string that represents the destination URI.</param>
        /// <param name="parameters">Parameters of the COPY operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Copy(string sourceUri, string destUri, CopyParameters parameters)
        {
            return Copy(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        /// <summary>
        /// Creates a duplicate of the source resource identified by the source URI in the destination resource identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="Uri"/>.</param>
        /// <param name="parameters">Parameters of the COPY operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
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
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(sourceUri, WebDavMethod.Copy, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source URI.</param>
        /// <param name="destUri">A string that represents the destination URI.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Move(string sourceUri, string destUri)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), new MoveParameters());
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="Uri"/>.</param>
        /// <returns>An instance of <see cref="WebDavResponse" /></returns>
        public Task<WebDavResponse> Move(Uri sourceUri, Uri destUri)
        {
            return Move(sourceUri, destUri, new MoveParameters());
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">A string that represents the source URI.</param>
        /// <param name="destUri">A string that represents the destination URI.</param>
        /// <param name="parameters">Parameters of the MOVE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Move(string sourceUri, string destUri, MoveParameters parameters)
        {
            return Move(CreateUri(sourceUri), CreateUri(destUri), parameters);
        }

        /// <summary>
        /// Moves the resource identified by the source URI to the destination identified by the destination URI.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destUri">The destination <see cref="Uri"/>.</param>
        /// <param name="parameters">Parameters of the MOVE operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public async Task<WebDavResponse> Move(Uri sourceUri, Uri destUri, MoveParameters parameters)
        {
            Guard.NotNull(sourceUri, "sourceUri");
            Guard.NotNull(destUri, "destUri");

            var headerBuilder = new HeaderBuilder()
                .Add(WebDavHeaders.Destination, GetAbsoluteUri(destUri).AbsoluteUri)
                .Add(WebDavHeaders.Overwrite, parameters.Overwrite ? "T" : "F");

            if (!string.IsNullOrEmpty(parameters.SourceLockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.SourceLockToken!));
            if (!string.IsNullOrEmpty(parameters.DestLockToken))
                headerBuilder.Add(WebDavHeaders.If, IfHeaderHelper.GetHeaderValue(parameters.DestLockToken!));

            var headers = headerBuilder.AddWithOverwrite(parameters.Headers).Build();
            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(sourceUri, WebDavMethod.Move, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Takes out a shared lock or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <returns>An instance of <see cref="LockResponse" />.</returns>
        public Task<LockResponse> Lock(string requestUri)
        {
            return Lock(CreateUri(requestUri), new LockParameters());
        }

        /// <summary>
        /// Takes out a shared lock or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <returns>An instance of <see cref="LockResponse" />.</returns>
        public Task<LockResponse> Lock(Uri requestUri)
        {
            return Lock(requestUri, new LockParameters());
        }

        /// <summary>
        /// Takes out a lock of any type or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the LOCK operation.</param>
        /// <returns>An instance of <see cref="LockResponse" />.</returns>
        public Task<LockResponse> Lock(string requestUri, LockParameters parameters)
        {
            return Lock(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Takes out a lock of any type or refreshes an existing lock of the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the LOCK operation.</param>
        /// <returns>An instance of <see cref="LockResponse" />.</returns>
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
            var requestParams = new RequestParameters { Headers = headers, Content = new StringContent(requestBody), ContentType = parameters.ContentType };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Lock, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return new LockResponse((int)response.StatusCode, response.ReasonPhrase);

            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _lockResponseParser!.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="lockToken">The resource lock token.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Unlock(string requestUri, string lockToken)
        {
            return Unlock(CreateUri(requestUri), new UnlockParameters(lockToken));
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="lockToken">The resource lock token.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Unlock(Uri requestUri, string lockToken)
        {
            return Unlock(requestUri, new UnlockParameters(lockToken));
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the UNLOCK operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public Task<WebDavResponse> Unlock(string requestUri, UnlockParameters parameters)
        {
            return Unlock(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Removes the lock identified by the lock token from the resource identified by the request URI.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the UNLOCK operation.</param>
        /// <returns>An instance of <see cref="WebDavResponse" />.</returns>
        public async Task<WebDavResponse> Unlock(Uri requestUri, UnlockParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");

            var headers = new HeaderBuilder()
                .Add(WebDavHeaders.LockToken, $"<{parameters.LockToken}>")
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestParams = new RequestParameters { Headers = headers };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Unlock, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            return new WebDavResponse((int)response.StatusCode, response.ReasonPhrase);
        }

        /// <summary>
        /// Executes a SEARCH operation.
        /// </summary>
        /// <param name="requestUri">A string that represents the request URI.</param>
        /// <param name="parameters">Parameters of the SEARCH operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public Task<PropfindResponse> Search(string requestUri, SearchParameters parameters)
        {
            return Search(CreateUri(requestUri), parameters);
        }

        /// <summary>
        /// Executes a SEARCH operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> to request.</param>
        /// <param name="parameters">Parameters of the SEARCH operation.</param>
        /// <returns>An instance of <see cref="PropfindResponse" />.</returns>
        public async Task<PropfindResponse> Search(Uri requestUri, SearchParameters parameters)
        {
            Guard.NotNull(requestUri, "requestUri");
            parameters.AssertParametersAreValid();

            var headers = new HeaderBuilder()
                .AddWithOverwrite(parameters.Headers)
                .Build();

            var requestBody = new StringContent(SearchRequestBuilder.BuildRequestBody(parameters));
            var requestParams = new RequestParameters { Headers = headers, Content = requestBody, ContentType = new MediaTypeHeaderValue("text/xml") };
            var response = await _dispatcher!.Send(requestUri, WebDavMethod.Search, requestParams, parameters.CancellationToken).ConfigureAwait(false);
            var responseContent = await ReadContentAsString(response.Content).ConfigureAwait(false);
            return _propfindResponseParser!.Parse(responseContent, (int)response.StatusCode, response.ReasonPhrase);
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
            return requestUri != null ? new Uri(requestUri, UriKind.RelativeOrAbsolute) : null!;
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
            if (uri == null && _dispatcher!.BaseAddress == null)
                throw CreateInvalidUriException();

            if (uri == null)
                return _dispatcher!.BaseAddress;

            if (uri.IsAbsoluteUri)
                return uri;

            if (_dispatcher!.BaseAddress == null)
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
