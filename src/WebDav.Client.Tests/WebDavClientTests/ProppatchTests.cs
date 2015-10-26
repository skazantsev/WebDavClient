using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Xml.Linq;
using WebDav.Client.Tests.TestDoubles;
using WebDav.Response;
using Xunit;

namespace WebDav.Client.Tests.WebDavClientTests
{
    public class ProppatchTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Proppatch("http://example.com", new ProppatchParameters());
            var response2 = await client.Proppatch(new Uri("http://example.com"), new ProppatchParameters());

            Assert.Equal(200, response1.StatusCode);
            Assert.Equal(200, response2.StatusCode);
        }

        [Fact]
        public async void When_RequestIsSuccessfull_Should_ParseResponse()
        {
            var dispatcher = Dispatcher.Mock("response", 207, "Multi-Status");
            var proppatchResponseParser = Substitute.For<IResponseParser<ProppatchResponse>>();
            var client = new WebDavClient()
                .SetWebDavDispatcher(dispatcher)
                .SetProppatchResponseParser(proppatchResponseParser);

            proppatchResponseParser.DidNotReceiveWithAnyArgs().Parse("", 0, "");
            await client.Proppatch("http://example", new ProppatchParameters());
            proppatchResponseParser.Received(1).Parse("response", 207, "Multi-Status");
        }

        [Fact]
        public async void When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.MockFaulted());
            var response = await client.Proppatch("http://example", new ProppatchParameters());
            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendProppatchRequest()
        {
            var requestUri = new Uri("http://example.com");
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await dispatcher.DidNotReceiveWithAnyArgs().Send(requestUri, Arg.Any<HttpMethod>(), new RequestParameters(), CancellationToken.None);
            await client.Proppatch(requestUri, new ProppatchParameters());
            await dispatcher.Received(1)
                .Send(requestUri, WebDavMethod.Proppatch, Arg.Is<RequestParameters>(x => !x.Headers.Any()), CancellationToken.None);
        }

        [Fact]
        public async void When_IsCalledWithCancellationToken_Should_SendRequestWithIt()
        {
            var cts = new CancellationTokenSource();
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Proppatch("http://example.com", new ProppatchParameters { CancellationToken = cts.Token });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is<RequestParameters>(x => !x.Headers.Any()), cts.Token);
        }

        [Fact]
        public async void When_IsCalledWithDefaultArguments_Should_SendPropertyUpdateRequest()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propertyupdate xmlns:D=""DAV:"" />";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Proppatch("http://example.com", new ProppatchParameters());
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_SetProperties_Should_IncludeThemInSetTag()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propertyupdate xmlns:D=""DAV:"">
  <D:set>
    <D:prop>
      <prop1>value1</prop1>
    </D:prop>
    <D:prop>
      <prop2>value2</prop2>
    </D:prop>
  </D:set>
</D:propertyupdate>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            var propertiesToSet = new Dictionary<XName, string>
            {
                {"prop1", "value1"},
                {"prop2", "value2"}
            };
            await client.Proppatch("http://example.com", new ProppatchParameters { PropertiesToSet = propertiesToSet });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_SetPropertiesWithNamespaces_Should_IncludeXmlnsTagAndUsePrefixes()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propertyupdate xmlns:D=""DAV:"" xmlns:X=""http://x.example.com/"" xmlns=""http://y.example.com/"">
  <D:set>
    <D:prop>
      <X:prop1>value1</X:prop1>
    </D:prop>
    <D:prop>
      <prop2>value2</prop2>
    </D:prop>
  </D:set>
</D:propertyupdate>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            var propertiesToSet = new Dictionary<XName, string>
            {
                {"{http://x.example.com/}prop1", "value1"},
                {"{http://y.example.com/}prop2", "value2"}
            };
            var ns = new[]
            {
                new NamespaceAttr("X", "http://x.example.com/"),
                new NamespaceAttr("http://y.example.com/")
            };
            await client.Proppatch("http://example.com", new ProppatchParameters { PropertiesToSet = propertiesToSet, Namespaces = ns});
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_RemoveProperties_Should_IncludeThemInRemoveTag()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propertyupdate xmlns:D=""DAV:"">
  <D:remove>
    <D:prop>
      <prop1 />
    </D:prop>
    <D:prop>
      <prop2 />
    </D:prop>
  </D:remove>
</D:propertyupdate>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            await client.Proppatch("http://example.com", new ProppatchParameters { PropertiesToRemove = new XName[] { "prop1", "prop2" } });
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }

        [Fact]
        public async void When_RemovePropertiesWithNamespaces_Should_IncludeXmlnsTagAndUsePrefixes()
        {
            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:propertyupdate xmlns:D=""DAV:"" xmlns:X=""http://x.example.com/"" xmlns=""http://y.example.com/"">
  <D:remove>
    <D:prop>
      <X:prop1 />
    </D:prop>
    <D:prop>
      <prop2 />
    </D:prop>
  </D:remove>
</D:propertyupdate>";
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            var propertiesToRemove = new XName[]
            {
                "{http://x.example.com/}prop1",
                "{http://y.example.com/}prop2"
            };
            var ns = new[]
            {
                new NamespaceAttr("X", "http://x.example.com/"),
                new NamespaceAttr("http://y.example.com/")
            };
            await client.Proppatch("http://example.com", new ProppatchParameters { PropertiesToRemove = propertiesToRemove, Namespaces = ns});
            await dispatcher.Received(1)
                .Send(Arg.Any<Uri>(), WebDavMethod.Proppatch, Arg.Is(Predicates.CompareRequestContent(expectedContent)), CancellationToken.None);
        }
    }
}
