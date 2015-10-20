using System.Linq;
using WebDav.Response;
using Xunit;

namespace WebDav.Client.Tests.Response
{
    public class PropfindResponseParserTests
    {
        [Fact]
        public void When_ResponseIsNull_Should_ReturnEmptyResourcesCollection()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(null, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_ResponseIsEmpty_Should_ReturnEmptyResourcesCollection()
        {
            const string htmlresponse = "";
            var parser = new PropfindResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_NotValidXml_Should_ReturnEmptyResourcesCollection()
        {
            const string htmlresponse = "<root></";
            var parser = new PropfindResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_EmptyMultiStatus_Should_ReturnEmptyResourcesCollection()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.EmptyMultiStatusResponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_ResponseHasNoProperties_Should_ReturnOneElementWithDefaultValues()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.ResponseWithoutProperties, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);

            Assert.Equal(1, response.Resources.Count);
            Assert.False(element.IsCollection);
            Assert.False(element.IsHidden);
            Assert.Empty(element.ActiveLocks);
            Assert.Null(element.ContentLanguage);
            Assert.Null(element.ContentLength);
            Assert.Null(element.ContentType);
            Assert.Null(element.CreationDate);
            Assert.Null(element.DisplayName);
            Assert.Null(element.ETag);
            Assert.Null(element.LastModifiedDate);
            Assert.Empty(element.Properties);
            Assert.Empty(element.PropertyStatuses);
            Assert.Equal("http://www.example.com/file", element.Uri);
        }

        [Fact]
        public void When_ResponseHasOneDeadProperty_Should_ReturnItsValue()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.ResponseWithOneFileProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);
            var property = element.Properties.ElementAt(0);

            Assert.Equal("testprop", property.Name);
            Assert.Equal("test", property.Value);
        }

        [Fact]
        public void When_ResponseHasOneDeadProperty_Should_ReturnItsStatus()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.ResponseWithOneFileProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);
            var status = element.PropertyStatuses.ElementAt(0);

            Assert.True(status.IsSuccessful);
            Assert.Equal(1, element.PropertyStatuses.Count);
            Assert.Equal(200, status.StatusCode);
            Assert.Equal("testprop", status.Name);
            Assert.Equal("HTTP/1.1 200 OK", status.Description);
        }

        [Fact]
        public void When_ResourceHasWrongUriFormat_Should_ReturnStilReturnIt()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.ResponseWithWrongUriFormat, 207, "Multi-Status");

            Assert.Equal("URI", response.Resources.ElementAt(0).Uri);
        }

        [Fact]
        public void When_PropertyStatusHasWrongFormat_Should_ReturnItAnyway()
        {
            var parser = new PropfindResponseParser();
            var response = parser.Parse(Responses.Propfind.ResponseWithWrongStatusFormat, 207, "Multi-Status");
            var status = response.Resources.ElementAt(0).PropertyStatuses.ElementAt(0);

            Assert.False(status.IsSuccessful);
            Assert.Equal(0, status.StatusCode);
            Assert.Equal("testprop", status.Name);
            Assert.Equal("STATUS", status.Description);
        }
    }
}
