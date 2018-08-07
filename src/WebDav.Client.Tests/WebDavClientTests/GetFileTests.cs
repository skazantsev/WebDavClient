using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class GetFileTests
    {
        [Fact]
        public async void When_GetRawFileIsCalled_Should_ProxyCallToGetFile()
        {
            var client = Substitute.ForPartsOf<WebDavClient>().SetWebDavDispatcher(Dispatcher.Mock());
            client.GetFile(Arg.Any<Uri>(), Arg.Any<bool>(), Arg.Any<GetFileParameters>())
                .Returns(new WebDavStreamResponse(new HttpResponseMessage { StatusCode = HttpStatusCode.OK }, null));

            await client.GetRawFile(new Uri("http://example.com/file"));
            await client.GetRawFile("http://example.com/file");
            await client.GetRawFile(new Uri("http://example.com/file"), new GetFileParameters());
            await client.GetRawFile("http://example.com/file", new GetFileParameters());

            await client.Received(4).GetFile(Arg.Is<Uri>(x => x.ToString() == "http://example.com/file"), false, Arg.Any<GetFileParameters>());
        }

        [Fact]
        public async void When_GetProcessedFileIsCalled_Should_ProxyCallToGetFile()
        {
            var client = Substitute.ForPartsOf<WebDavClient>().SetWebDavDispatcher(Dispatcher.Mock());
            client.GetFile(Arg.Any<Uri>(), Arg.Any<bool>(), Arg.Any<GetFileParameters>())
                .Returns(new WebDavStreamResponse(new HttpResponseMessage { StatusCode = HttpStatusCode.OK }, null));

            await client.GetProcessedFile(new Uri("http://example.com/file"));
            await client.GetProcessedFile("http://example.com/file");
            await client.GetProcessedFile(new Uri("http://example.com/file"), new GetFileParameters());
            await client.GetProcessedFile("http://example.com/file", new GetFileParameters());

            await client.Received(4).GetFile(Arg.Is<Uri>(x => x.ToString() == "http://example.com/file"), true, Arg.Any<GetFileParameters>());
        }

        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.GetFile(new Uri("http://example.com/file"), false, new GetFileParameters());
            var response2 = await client.GetFile(new Uri("http://example.com/file"), true, new GetFileParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response1 = await client.GetFile(new Uri("http://example.com/file"), false, new GetFileParameters());
            var response2 = await client.GetFile(new Uri("http://example.com/file"), true, new GetFileParameters());

            Assert.Equal(500, response1.StatusCode);
            Assert.Equal(500, response2.StatusCode);
        }

        [Fact]
        public async void When_GetFile_Should_SendGetRequest()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.GetFile(requestUri, false, new GetFileParameters());
            await client.GetFile(requestUri, true, new GetFileParameters());
            await dispatcher.Received(2)
                .Send(requestUri, HttpMethod.Get, Arg.Is<RequestParameters>(x => x.Content == null), CancellationToken.None, HttpCompletionOption.ResponseHeadersRead);
        }

        [Fact]
        public async void When_IsCalledWithTranslateOff_Should_SendTranslateHeaderEqualsF()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.GetFile(requestUri, false, new GetFileParameters());
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Get, Arg.Is(Predicates.CompareHeader("Translate", "f")), CancellationToken.None, HttpCompletionOption.ResponseHeadersRead);
        }

        [Fact]
        public async void When_IsCalledWithTranslateOn_Should_SendTranslateHeaderEqualsT()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.GetFile(requestUri, true, new GetFileParameters());
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Get, Arg.Is(Predicates.CompareHeader("Translate", "t")), CancellationToken.None, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
