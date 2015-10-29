using NSubstitute;
using System;
using System.Net.Http;
using System.Threading;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class UnlockTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Unlock("http://example.com/file", "lock-token");
            var response2 = await client.Unlock(new Uri("http://example.com/file"), "lock-token");
            var response3 = await client.Unlock("http://example.com/file", new UnlockParameters("lock-token"));
            var response4 = await client.Unlock(new Uri("http://example.com/file"), new UnlockParameters("lock-token"));

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Unlock("http://example.com/file", "lock-token");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendUnlockRequest()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Unlock(requestUri, "lock-token");
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Unlock, Arg.Is<RequestParameters>(x => x.Content == null), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Unlock("http://example.com/file", new UnlockParameters("lock-token") { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Unlock, Arg.Is<RequestParameters>(x => x.Content == null), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithLockToken_Should_SendLockTokenHeader()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);

            await client.Unlock(requestUri, "urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4");
            await client.Unlock(requestUri, new UnlockParameters("urn:uuid:f81d4fae-7dec-11d0-a765-00a0c91e6bf6"));

            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Unlock, Arg.Is(Predicates.CompareHeader("Lock-Token", "<urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4>")), CancellationToken.None);

            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Unlock, Arg.Is(Predicates.CompareHeader("Lock-Token", "<urn:uuid:f81d4fae-7dec-11d0-a765-00a0c91e6bf6>")), CancellationToken.None);
        }
    }
}
