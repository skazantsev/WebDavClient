using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class MkcolTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Mkcol("http://example.com/new");
            var response2 = await client.Mkcol(new Uri("http://example.com/new"));
            var response3 = await client.Mkcol("http://example.com/new", new MkColParameters());
            var response4 = await client.Mkcol(new Uri("http://example.com/new"), new MkColParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Mkcol("http://example.com/new");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendMkcolRequest()
        {
            var requestUri = new Uri("http://example.com/new");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Mkcol(requestUri);
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Mkcol, Arg.Is<RequestParameters>(x => !x.Headers.Any() && x.Content == null), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Mkcol("http://example.com/new", new MkColParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Mkcol, Arg.Is<RequestParameters>(x => !x.Headers.Any() && x.Content == null), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithLockToken_Should_SetIfHeader()
        {
            var requestUri = new Uri("http://example.com/new");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Mkcol(requestUri, new MkColParameters { LockToken = "urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4" });
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Mkcol, Arg.Is(Predicates.CompareHeader("If", "(<urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4>)")), CancellationToken.None);
        }
    }
}
