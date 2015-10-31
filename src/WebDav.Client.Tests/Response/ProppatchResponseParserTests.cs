using System.Linq;
using WebDav.Response;
using Xunit;

namespace WebDav.Client.Tests.Response
{
    public class ProppatchResponseParserTests
    {
        [Fact]
        public void When_ResponseIsNull_Should_ReturnEmptyPropertyStatusesCollection()
        {
            var parser = new ProppatchResponseParser();
            var response = parser.Parse(null, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.PropertyStatuses);
        }

        [Fact]
        public void When_ResponseIsEmpty_Should_ReturnEmptyPropertyStatusesCollection()
        {
            const string htmlresponse = "";
            var parser = new ProppatchResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.PropertyStatuses);
        }

        [Fact]
        public void When_NotValidXml_Should_ReturnEmptyPropertyStatusesCollection()
        {
            const string htmlresponse = "<root></";
            var parser = new ProppatchResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.PropertyStatuses);
        }

        [Fact]
        public void When_EmptyMultiStatus_Should_ReturnEmptyResourcesCollection()
        {
            var parser = new ProppatchResponseParser();
            var response = parser.Parse(Responses.Common.EmptyMultiStatusResponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.PropertyStatuses);
        }

        [Fact]
        public void When_ResponseHasProperties_Should_ReturnTheirStatus()
        {
            var parser = new ProppatchResponseParser();
            var response = parser.Parse(Responses.Proppatch.ResponseWithDifferentStatuses, 207, "Multi-Status");
            var status1 = response.PropertyStatuses.ElementAt(0);
            var status2 = response.PropertyStatuses.ElementAt(1);

            Assert.Equal(2, response.PropertyStatuses.Count);

            Assert.Equal(200, status1.StatusCode);
            Assert.True(status1.IsSuccessful);
            Assert.Equal("{DAV:}getcontenttype", status1.Name);
            Assert.Equal("HTTP/1.1 200 OK", status1.Description);

            Assert.Equal(424, status2.StatusCode);
            Assert.False(status2.IsSuccessful);
            Assert.Equal("{http://ns.example.com/}myprop", status2.Name);
            Assert.Equal("HTTP/1.1 424 Failed Dependency", status2.Description);
        }
    }
}
