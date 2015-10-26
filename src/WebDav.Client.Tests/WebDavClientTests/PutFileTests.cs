using NSubstitute;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class PutFileTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());

            var response1 = await client.PutFile("http://example.com/file", stream);
            var response2 = await client.PutFile(new Uri("http://example.com/file"), stream);
            var response3 = await client.PutFile("http://example.com/file", stream, "text/xml");
            var response4 = await client.PutFile(new Uri("http://example.com/file"), stream, "text/xml");
            var response5 = await client.PutFile(new Uri("http://example.com/file"), stream, new PutFileParameters());
            var response6 = await client.PutFile(new Uri("http://example.com/file"), stream, new PutFileParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
            Assert.Equal(200, response5.StatusCode);
            Assert.Equal(200, response6.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.PutFile("http://example.com/file", stream);
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalled_Should_SendPutRequestWithContent()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.PutFile(requestUri, stream);
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Put, Arg.Is(Predicates.CompareRequestContent("<content/>")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.PutFile("http://example.com/file", stream, new PutFileParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), HttpMethod.Put, Arg.Is(Predicates.CompareRequestContent("<content/>")), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithLockToken_Should_SetIfHeader()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.PutFile(requestUri, stream, new PutFileParameters { LockToken = "urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4" });
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Put, Arg.Is(Predicates.CompareHeader("If", "(<urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4>)")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithContentType_Should_SetPassItToDispatcher()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("<content/>"));
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.PutFile(requestUri, stream, new PutFileParameters { ContentType = "text/xml" });
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Put, Arg.Is<RequestParameters>(x => x.ContentType == "text/xml"), CancellationToken.None);
        }
    }
}
