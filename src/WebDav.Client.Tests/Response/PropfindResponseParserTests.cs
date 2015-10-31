using System;
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
            var parser = new PropfindResponseParser(new LockResponseParser());
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
            var parser = new PropfindResponseParser(new LockResponseParser());
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
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_EmptyMultiStatus_Should_ReturnEmptyResourcesCollection()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Common.EmptyMultiStatusResponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.Resources);
        }

        [Fact]
        public void When_ResponseHasNoProperties_Should_ReturnOneElementWithDefaultValues()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
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
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.FileResponseWithOneProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);
            var property = element.Properties.ElementAt(0);

            Assert.Equal("testprop", property.Name);
            Assert.Equal("test", property.Value);
        }

        [Fact]
        public void When_ResponseHasOneDeadProperty_Should_ReturnItsStatus()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.FileResponseWithOneProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);
            var status = element.PropertyStatuses.ElementAt(0);

            Assert.True(status.IsSuccessful);
            Assert.Equal(1, element.PropertyStatuses.Count);
            Assert.Equal(200, status.StatusCode);
            Assert.Equal("testprop", status.Name);
            Assert.Equal("HTTP/1.1 200 OK", status.Description);
        }

        [Fact]
        public void When_ResponseHasStandardProperties_Should_FillWebDavResourceProperties()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.FileResponseWithStandardProperties, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);
            var @lock = element.ActiveLocks.ElementAt(0);

            Assert.Equal(12, element.Properties.Count);
            Assert.Equal(12, element.PropertyStatuses.Count);

            Assert.False(element.IsCollection);
            Assert.False(element.IsHidden);

            Assert.Equal(1, element.ActiveLocks.Count);
            Assert.Equal(ApplyTo.Lock.ResourceOnly, @lock.ApplyTo);
            Assert.Equal(LockScope.Exclusive, @lock.LockScope);
            Assert.Equal("urn:uuid:f81de2ad-7f3d-a1b2-4f3c-00a0c91a9d76", @lock.LockToken);
            Assert.Equal("@Me", @lock.Owner.Value);
            Assert.IsType(typeof(PrincipalLockOwner), @lock.Owner);
            Assert.Equal("http://www.example.com/1.txt", @lock.LockRoot);
            Assert.Equal(null, @lock.Timeout);

            Assert.Equal("en", element.ContentLanguage);
            Assert.Equal(4, element.ContentLength.Value);
            Assert.Equal("text/plain", element.ContentType);
            Assert.Equal(new DateTime(2015, 8, 19, 22, 59, 25, 160, DateTimeKind.Utc), element.CreationDate.Value.ToUniversalTime());
            Assert.Equal("1.txt", element.DisplayName);
            Assert.Equal("\"c5f02718ae6d01:0\"", element.ETag);
            Assert.Equal(new DateTime(2015, 9, 3, 20, 49, 19, 0, DateTimeKind.Utc), element.LastModifiedDate.Value.ToUniversalTime());
            Assert.Equal(12, element.Properties.Count);
            Assert.Equal(12, element.PropertyStatuses.Count);
            Assert.Equal("http://www.example.com/1.txt", element.Uri);
        }

        [Fact]
        public void When_PropertyStatusOutOfRange2XX_Should_NotSuccessfullStatus()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.FileResponse403Status, 207, "Multi-Status");
            var status = response.Resources.ElementAt(0).PropertyStatuses.ElementAt(0);

            Assert.False(status.IsSuccessful);
            Assert.Equal(403, status.StatusCode);
            Assert.Equal("HTTP/1.1 403 Forbidden", status.Description);
        }

        [Fact]
        public void When_IsCollectionPropertyEqualsZero_Should_ReturnFile()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithIsCollectionZeroProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);

            Assert.False(element.IsCollection);
        }

        [Fact]
        public void When_IsCollectionPropertyEqualsNotZero_Should_ReturnCollection()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithIsCollectionOneProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);

            Assert.True(element.IsCollection);
        }

        [Fact]
        public void When_ResourceTypePropertyIsCollection_Should_ReturnCollection()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithResourceTypeCollectionProperty, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);

            Assert.True(element.IsCollection);
        }

        [Fact]
        public void When_PropertyHasComplexValue_Should_ReturnItsValueAsString()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithComplexProperties, 207, "Multi-Status");
            var property = response.Resources.ElementAt(0).Properties.ElementAt(0);

            Assert.Equal("author", property.Name);
            Assert.Equal("<Name>J.J. Johnson</Name>", property.Value);
        }

        [Fact]
        public void When_ResponseHasSeveralResources_Should_ParseThemAll()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithTwoResources, 207, "Multi-Status");

            Assert.Equal(2, response.Resources.Count);
        }

        [Fact]
        public void When_ResponseHasPropertiesWithXmlNamespaces_Should_ReturnPropertyNameWithNamespace()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithPropertyWithXmlNamespace, 207, "Multi-Status");
            var property = response.Resources.ElementAt(0).Properties.ElementAt(0);

            Assert.Equal("{http://ns.example.com/boxschema/}author", property.Name);
            Assert.Equal("author", property.Name.LocalName);
            Assert.Equal("http://ns.example.com/boxschema/", property.Name.Namespace);
        }

        [Fact]
        public void When_ResponseHasRepeatedProperties_Should_TakeFirst()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.FileResponseWithMultipleSimilarProperties, 207, "Multi-Status");
            var property = response.Resources.ElementAt(0).Properties.ElementAt(0);

            Assert.Equal("v1", property.Value);
        }

        [Fact]
        public void When_ResourceHasWrongUriFormat_Should_ReturnStilReturnIt()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithWrongUriFormat, 207, "Multi-Status");

            Assert.Equal("URI", response.Resources.ElementAt(0).Uri);
        }

        [Fact]
        public void When_PropertyStatusHasWrongFormat_Should_ReturnItAnyway()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithWrongStatusFormat, 207, "Multi-Status");
            var status = response.Resources.ElementAt(0).PropertyStatuses.ElementAt(0);

            Assert.False(status.IsSuccessful);
            Assert.Equal(0, status.StatusCode);
            Assert.Equal("testprop", status.Name);
            Assert.Equal("STATUS", status.Description);
        }

        [Fact]
        public void When_ResponseDoesNotHaveXmlDeclaration_Should_Handle()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithoutXmlDeclaration, 207, "Multi-Status");

            Assert.Equal(1, response.Resources.Count);
        }

        [Fact]
        public void When_ResponseHasIncorrectCreationDateFormat_Should_ReturnNull()
        {
            var parser = new PropfindResponseParser(new LockResponseParser());
            var response = parser.Parse(Responses.Propfind.ResponseWithIncorectCreationDateFormat, 207, "Multi-Status");
            var element = response.Resources.ElementAt(0);

            Assert.Null(element.CreationDate);
        }
    }
}
