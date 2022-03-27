using System;
using System.Collections.Generic;
using System.Text;
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
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Search(new Uri("http://example.com/"), new Request.SearchParameters()
            {
                SearchPath = "/root/",
                SearchKeyword = "test%",
                SelectProperties = new[]
                           {
                                new XElement("{DAV:}displayname"),
                                new XElement("{DAV:}getcontenttype")
                           },
                WhereProperties = new[]
                           {
                                new XElement("{DAV:}displayname")
                           }
            });

            Assert.Equal(200, response1.StatusCode);
        }

        [Fact]
        public async void When_RequestIsBadRequest_Should_ReturnStatusCode400()
        {
            var client = new WebDavClient().SetWebDavDispatcher(Dispatcher.Mock());
            var response1 = await client.Search(new Uri("http://example.com/"), new Request.SearchParameters());
            Assert.Equal(400, response1.StatusCode);
        }

    }
}
