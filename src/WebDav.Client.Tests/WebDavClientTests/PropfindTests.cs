using NSubstitute;
using System;
using System.Net.Http;
using System.Threading;
using System.Xml.Linq;
using WebDav.Client.Tests.TestDoubles;
using WebDav.Response;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class PropfindTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Propfind("http://example.com");
            var response2 = await client.Propfind(new Uri("http://example.com"));
            var response3 = await client.Propfind("http://example.com", new PropfindParameters());
            var response4 = await client.Propfind(new Uri("http://example.com"), new PropfindParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
            Assert.Equal(200, response3.StatusCode);
            Assert.Equal(200, response4.StatusCode);
        }

        [Fact]
        public async void When_RequestIsSuccessfull_Should_ParseResponse()
        {
            var dispatcher = Dispatcher.Mock("response", 207, "Multi-Status");
            var propfindResponseParser = Substitute.For<IResponseParser<PropfindResponse>>();
            var client = new WebDavClient()
                .SetWebDavDispatcher(dispatcher)
                .SetPropfindResponseParser(propfindResponseParser);

            propfindResponseParser.DidNotReceiveWithAnyArgs().Parse("", 0, "");
            await client.Propfind("http://example");
            propfindResponseParser.Received(1).Parse("response", 207, "Multi-Status");
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Propfind("http://example.com");
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendPropfindRequest()
        {
            var requestUri = new Uri("http://example.com");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Propfind(requestUri);
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Propfind, Arg.Is(Predicates.CompareHeader("Depth", "1")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsAppliedToResourceAndAncestors_Should_SendDepthHeaderEqualsInfinity()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com", new PropfindParameters { ApplyTo = ApplyTo.Propfind.ResourceAndAncestors });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareHeader("Depth", "infinity")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsAppliedToResourceAndChildren_Should_SendDepthHeaderEqualsOne()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com", new PropfindParameters { ApplyTo = ApplyTo.Propfind.ResourceAndChildren });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareHeader("Depth", "1")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsAppliedToResourceOnly_Should_SendDepthHeaderEqualsZero()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com", new PropfindParameters { ApplyTo = ApplyTo.Propfind.ResourceOnly });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareHeader("Depth", "0")), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com", new PropfindParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareHeader("Depth", "1")), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendAllPropRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com");
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCustomProperties_Should_IncludeThemInRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
  <D:include>
    <myprop1 />
    <myprop2 />
  </D:include>
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com",
                new PropfindParameters
                {
                    CustomProperties = new XName[] { "myprop1", "myprop2" }
                });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_CustomPropertiesHaveNamespaces_Should_IncludeThemInRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
  <D:include>
    <myprop1 xmlns=""http://ns1.example.com"" />
    <myprop2 xmlns=""http://ns2.example.com"" />
  </D:include>
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com",
                new PropfindParameters
                {
                    CustomProperties = new XName[] { "{http://ns1.example.com}myprop1", "{http://ns2.example.com}myprop2" }
                });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithDefaultNamespace_Should_IncludeItInRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
  <D:include xmlns=""http://ns.example.com"">
    <myprop1 />
    <myprop2 />
  </D:include>
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com",
                new PropfindParameters
                {
                    CustomProperties = new XName[] { "{http://ns.example.com}myprop1", "{http://ns.example.com}myprop2" },
                    Namespaces = new[] { new NamespaceAttr("http://ns.example.com") }
                });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithMoreThanOneDefaultNamespace_Should_UseTheLastOne()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
  <D:include xmlns=""http://ns2.example.com"">
    <myprop />
  </D:include>
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com",
                new PropfindParameters
                {
                    CustomProperties = new XName[] { "{http://ns2.example.com}myprop" },
                    Namespaces = new[] { new NamespaceAttr("http://ns1.example.com"), new NamespaceAttr("http://ns2.example.com") }
                });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithPrefixedNamespaces_Should_IncludeThemInRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propfind xmlns:D=""DAV:"">
  <D:allprop />
  <D:include xmlns:P1=""http://p1.example.com"" xmlns:P2=""http://p2.example.com"">
    <P1:myprop1 />
    <P2:myprop2 />
  </D:include>
</D:propfind>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Propfind("http://example.com",
                new PropfindParameters
                {
                    CustomProperties = new XName[] { "{http://p1.example.com}myprop1", "{http://p2.example.com}myprop2" },
                    Namespaces = new[] { new NamespaceAttr("P1", "http://p1.example.com"), new NamespaceAttr("P2", "http://p2.example.com") }
                });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Propfind, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }
    }
}
