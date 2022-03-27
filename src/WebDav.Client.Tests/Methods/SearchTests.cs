using NSubstitute;
using System;
using System.Threading;
using System.Xml.Linq;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.Methods
{
    public class SearchTests
    {
        [Fact]
        public async void When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            var response = await client.Search("http://example.com/", new SearchParameters
            {
                Scope = "/",
                SearchProperty = "{DAV:}displayname",
                SearchKeyword = "test%"
            });

            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async void When_PassingNoSelectProperties_Should_IncludeAllProp()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            await client.Search("http://example.com/", new SearchParameters()
            {
                Scope = "/root/",
                SearchProperty = "{DAV:}displayname",
                SearchKeyword = "test%"
            });

            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:searchrequest xmlns:D=""DAV:"">
  <D:basicsearch>
    <D:select>
      <D:allprop />
    </D:select>
    <D:from>
      <D:scope>
        <D:href>/root/</D:href>
        <D:depth>infinity</D:depth>
      </D:scope>
    </D:from>
    <D:where>
      <D:like>
        <D:prop>
          <D:displayname />
        </D:prop>
        <D:literal>test%</D:literal>
      </D:like>
    </D:where>
  </D:basicsearch>
</D:searchrequest>";
            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Search,
                Arg.Is(Predicates.CompareRequestContent(expectedContent)),
                CancellationToken.None
            );
        }

        [Fact]
        public async void When_PassingSelectProperties_Should_IncludeThem()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            await client.Search("http://example.com/", new SearchParameters()
            {
                Scope = "/root/",
                SearchProperty = "{DAV:}displayname",
                SearchKeyword = "test%",
                SelectProperties = new XName[]
                {
                    "{DAV:}displayname",
                    "{DAV:}getcontenttype"
                }
            });

            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:searchrequest xmlns:D=""DAV:"">
  <D:basicsearch>
    <D:select>
      <D:prop>
        <D:displayname />
        <D:getcontenttype />
      </D:prop>
    </D:select>
    <D:from>
      <D:scope>
        <D:href>/root/</D:href>
        <D:depth>infinity</D:depth>
      </D:scope>
    </D:from>
    <D:where>
      <D:like>
        <D:prop>
          <D:displayname />
        </D:prop>
        <D:literal>test%</D:literal>
      </D:like>
    </D:where>
  </D:basicsearch>
</D:searchrequest>";
            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Search,
                Arg.Is(Predicates.CompareRequestContent(expectedContent)),
                CancellationToken.None
            );
        }

        [Fact]
        public async void Should_SupportPropertiesWithDifferentNamespaces()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            var response = await client.Search("http://example.com/", new SearchParameters()
            {
                Scope = "/root/",
                SearchProperty = "{NS1}prop1",
                SearchKeyword = "test%",
                SelectProperties = new XName[]
                {
                    "{NS1}prop1",
                    "{http://ns2.example.com}prop2",
                    "{DEFAULT}prop3",
                    "{DAV:}prop4"
                },
                Namespaces = new []
                {
                    new NamespaceAttr("NS1", "NS1"),
                    new NamespaceAttr("NS2", "http://ns2.example.com"),
                    new NamespaceAttr("DEFAULT")
                }
            });

            Assert.Equal(200, response.StatusCode);

            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:searchrequest xmlns:D=""DAV:"" xmlns:NS1=""NS1"" xmlns:NS2=""http://ns2.example.com"" xmlns=""DEFAULT"">
  <D:basicsearch>
    <D:select>
      <D:prop>
        <NS1:prop1 />
        <NS2:prop2 />
        <prop3 />
        <D:prop4 />
      </D:prop>
    </D:select>
    <D:from>
      <D:scope>
        <D:href>/root/</D:href>
        <D:depth>infinity</D:depth>
      </D:scope>
    </D:from>
    <D:where>
      <D:like>
        <D:prop>
          <NS1:prop1 />
        </D:prop>
        <D:literal>test%</D:literal>
      </D:like>
    </D:where>
  </D:basicsearch>
</D:searchrequest>";
            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Search,
                Arg.Is(Predicates.CompareRequestContent(expectedContent)),
                CancellationToken.None
            );
        }
    }
}
