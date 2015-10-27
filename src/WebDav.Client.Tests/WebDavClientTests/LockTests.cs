using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using NSubstitute;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class LockTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Lock("http://example.com/new");
            var response2 = await client.Lock(new Uri("http://example.com/new"));
            var response3 = await client.Lock("http://example.com/new", new LockParameters());
            var response4 = await client.Lock(new Uri("http://example.com/new"), new LockParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Lock("http://example.com/new");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalled_Should_SendLockRequest()
        {
            var requestUri = new Uri("http://example.com/new");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Lock(requestUri);
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Lock, Arg.Any<RequestParameters>(), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendNoHeaders()
        {
            var requestUri = new Uri("http://example.com/new");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Lock(requestUri);
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Lock, Arg.Is<RequestParameters>(x => !x.Headers.Any()), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com/new", new LockParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Any<RequestParameters>(), cts.Token);
        }

        [Fact]
        public async void When_IsAppliedToResourceAndAncestors_Should_SendDepthHeaderEqualsInfinity()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { ApplyTo = ApplyTo.Lock.ResourceAndAncestors });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareHeader("Depth", "infinity")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsAppliedToResourceOnly_Should_SendDepthHeaderEqualsZero()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { ApplyTo = ApplyTo.Lock.ResourceOnly });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareHeader("Depth", "0")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithTimeout_Should_SendTimeoutHeaderInSeconds()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { Timeout = TimeSpan.FromMinutes(2) });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareHeader("Timeout", "Second-120")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendLockInfo()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:lockinfo xmlns:D=""DAV:"">
  <D:lockscope>
    <D:shared />
  </D:lockscope>
  <D:locktype>
    <D:write />
  </D:locktype>
</D:lockinfo>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com");
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithLockScopeShared_Should_AddSharedLockScopeToContent()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:lockinfo xmlns:D=""DAV:"">
  <D:lockscope>
    <D:shared />
  </D:lockscope>
  <D:locktype>
    <D:write />
  </D:locktype>
</D:lockinfo>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { LockScope = LockScope.Shared});
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithLockScopeExclusive_Should_AddExclusiveLockScopeToContent()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:lockinfo xmlns:D=""DAV:"">
  <D:lockscope>
    <D:exclusive />
  </D:lockscope>
  <D:locktype>
    <D:write />
  </D:locktype>
</D:lockinfo>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { LockScope = LockScope.Exclusive });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithPrincipalLockOwner_Should_AddOwnerToContent()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:lockinfo xmlns:D=""DAV:"">
  <D:lockscope>
    <D:shared />
  </D:lockscope>
  <D:locktype>
    <D:write />
  </D:locktype>
  <D:owner>James Bond</D:owner>
</D:lockinfo>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { Owner = new PrincipalLockOwner("James Bond") });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithUriLockOwner_Should_AddOwnerToContent()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:lockinfo xmlns:D=""DAV:"">
  <D:lockscope>
    <D:shared />
  </D:lockscope>
  <D:locktype>
    <D:write />
  </D:locktype>
  <D:owner>
    <D:href>http://example.com/owner</D:href>
  </D:owner>
</D:lockinfo>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Lock("http://example.com", new LockParameters { Owner = new UriLockOwner("http://example.com/owner") });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Lock, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }
    }
}
