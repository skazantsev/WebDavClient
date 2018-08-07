﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class DeleteTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Delete("http://example.com/file");
            var response2 = await client.Delete(new Uri("http://example.com/file"));
            var response3 = await client.Delete("http://example.com/file", new DeleteParameters());
            var response4 = await client.Delete(new Uri("http://example.com/file"), new DeleteParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Delete("http://example.com/file");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendDeleteRequest()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Delete(requestUri);
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Delete, Arg.Is<RequestParameters>(x => !x.Headers.Any() && x.Content == null), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Delete("http://example.com/file", new DeleteParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), HttpMethod.Delete, Arg.Is<RequestParameters>(x => !x.Headers.Any() && x.Content == null), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithLockToken_Should_SetIfHeader()
        {
            var requestUri = new Uri("http://example.com/file");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Delete(requestUri, new DeleteParameters { LockToken = "urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4" });
            await dispatcher.Received(1)
                .Send(requestUri, HttpMethod.Delete, Arg.Is(Predicates.CompareHeader("If", "(<urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4>)")), CancellationToken.None);
        }
    }
}
