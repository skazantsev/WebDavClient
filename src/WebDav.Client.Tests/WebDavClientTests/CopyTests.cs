using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class CopyTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Copy("http://example.com/old", "http://example.com/new");
            var response2 = await client.Copy(new Uri("http://example.com/old"), new Uri("http://example.com/new"));
            var response3 = await client.Copy("http://example.com/old", "http://example.com/new", new CopyParameters());
            var response4 = await client.Copy(new Uri("http://example.com/old"), new Uri("http://example.com/new"), new CopyParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Copy("http://example.com/old", "http://example.com/new");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalled_Should_SendCopyRequest()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"));
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(CheckCopyRequestParameters()), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalled_Should_SendDestinationHeader()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"));
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Destination", "http://example.com/new")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithDefaultParameters_Should_SendOverwriteHeaderEqualsT()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"));
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Overwrite", "T")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithDefaultParameters_Should_SendDepthHeaderEqualsInfinity()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"));
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Depth", "infinity")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithOverwriteOff_Should_SendOverwriteHeaderEqualsF()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"), new CopyParameters { Overwrite = false });
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Overwrite", "F")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithOverwriteOn_Should_SendOverwriteHeaderEqualsT()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"), new CopyParameters { Overwrite = true });
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Overwrite", "T")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Copy("http://example.com/old", "http://example.com/new", new CopyParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Copy, Arg.Is(CheckCopyRequestParameters()), cts.Token);
        }

        [Fact]
        public async void When_IsAppliedToResourceAndAncestors_Should_SendDepthHeaderEqualsInfinity()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"), new CopyParameters { ApplyTo = ApplyTo.Copy.ResourceAndAncestors});
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Depth", "infinity")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsAppliedToResourceOnly_Should_SendDepthHeaderEqualsZero()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"), new CopyParameters { ApplyTo = ApplyTo.Copy.ResourceOnly });
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("Depth", "0")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithLockToken_Should_SetIfHeader()
        {
            var sourceUri = new Uri("http://example.com/old");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(sourceUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Copy(sourceUri, new Uri("http://example.com/new"), new CopyParameters { DestLockToken = "urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4" });
            await dispatcher.Received(1)
                .Send(sourceUri, WebDavMethod.Copy, Arg.Is(Predicates.CompareHeader("If", "(<urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4>)")), CancellationToken.None);
        }

        internal Expression<Predicate<RequestParameters>> CheckCopyRequestParameters()
        {
            return x =>
                x.Headers.Any(h => h.Key == "Destination") &&
                x.Headers.Any(h => h.Key == "Depth") &&
                x.Headers.Any(h => h.Key == "Overwrite") &&
                x.Content == null;
        }
    }
}
