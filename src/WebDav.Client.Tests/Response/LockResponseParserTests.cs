using System.Linq;
using WebDav.Response;
using Xunit;

namespace WebDav.Client.Tests.Response
{
    public class LockResponseParserTests
    {
        [Fact]
        public void When_ResponseIsNull_Should_ReturnEmptyLocksCollection()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(null, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.ActiveLocks);
        }

        [Fact]
        public void When_ResponseIsEmpty_Should_ReturnEmptyLocksCollection()
        {
            const string htmlresponse = "";
            var parser = new LockResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.ActiveLocks);
        }

        [Fact]
        public void When_NotValidXml_Should_ReturnEmptyLocksCollection()
        {
            const string htmlresponse = "<root></";
            var parser = new LockResponseParser();
            var response = parser.Parse(htmlresponse, 207, "Multi-Status");

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Empty(response.ActiveLocks);
        }

        [Fact]
        public void When_ResponseHasEmptyActiveLock_Should_ReturnALockWithDefaultProperties()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithEmptyActiveLock, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Equal(207, response.StatusCode);
            Assert.Equal("Multi-Status", response.Description);
            Assert.True(response.IsSuccessful);
            Assert.Equal(1, response.ActiveLocks.Count);

            Assert.Null(activeLock.ApplyTo);
            Assert.Null(activeLock.LockScope);
            Assert.Null(activeLock.LockToken);
            Assert.Null(activeLock.Owner);
            Assert.Null(activeLock.LockRoot);
            Assert.Null(activeLock.Timeout);
        }

        [Fact]
        public void When_ResponseHasLockScopeExclusive_Should_ReturnExclusiveLock()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithExclusiveLockScope, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Equal(LockScope.Exclusive, activeLock.LockScope);
        }

        [Fact]
        public void When_ResponseHasLockScopeShared_Should_ReturnSharedLock()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithSharedLockScope, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.LockScope);
            Assert.Equal(LockScope.Shared, activeLock.LockScope);
        }

        [Fact]
        public void When_ResponseHasWrongLockScopeValue_Should_ReturnNullLockScope()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithWrongLockScopeValue, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Null(activeLock.LockScope);
        }

        [Fact]
        public void When_ResponseHasDepthInfinity_Should_ReturnLockAppliedToResourceAndAncestors()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithDepthEqualsInfinity, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.ApplyTo);
            Assert.Equal(ApplyTo.Lock.ResourceAndAncestors, activeLock.ApplyTo);
        }

        [Fact]
        public void When_ResponseHasDepthInfinity_Should_ReturnLockAppliedToResourceOnly()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithDepthEqualsZero, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.ApplyTo);
            Assert.Equal(ApplyTo.Lock.ResourceOnly, activeLock.ApplyTo);
        }

        [Fact]
        public void When_ResponseHasWrongDepthValue_Should_ReturnLockAppliedToResourceAndAncestors()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithWrongDepthValue, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.ApplyTo);
            Assert.Equal(ApplyTo.Lock.ResourceAndAncestors, activeLock.ApplyTo);
        }

        [Fact]
        public void When_ResponseHasHrefOwner_Should_ReturnLockWithUriOwner()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithHrefOwner, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.Owner);
            Assert.IsType(typeof (UriLockOwner), activeLock.Owner);
            Assert.Equal("http://example.org/~ejw/contact.html", activeLock.Owner.Value);
        }

        [Fact]
        public void When_ResponseHasOwnerValue_Should_ReturnLockWithPrincipalOwner()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithPrincipalOwner, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.Owner);
            Assert.IsType(typeof(PrincipalLockOwner), activeLock.Owner);
            Assert.Equal("Chuck Norris", activeLock.Owner.Value);
        }

        [Fact]
        public void When_ResponseHasTimeout_Should_ReturnTimeoutAsTimeSpan()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithTimeout, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.NotNull(activeLock.Timeout);
            Assert.Equal(119, activeLock.Timeout.Value.TotalSeconds);
        }

        [Fact]
        public void When_ResponseHasWrongFormattedTimeout_Should_ReturnNullTimeout()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithWrongTimeoutFormat, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Null(activeLock.Timeout);
        }

        [Fact]
        public void When_ResponseHasLockToken_Should_ReturnLockWithIt()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithLockToken, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Equal("urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4", activeLock.LockToken);
        }

        [Fact]
        public void When_ResponseHasLockRoot_Should_ReturnLockWithIt()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithLockRoot, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Equal("http://example.com/workspace/webdav/proposal.doc", activeLock.LockRoot);
        }

        [Fact]
        public void When_ResponseHasStandardProperties_Should_ParseIt()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithStandardProperties, 207, "Multi-Status");
            var activeLock = response.ActiveLocks.ElementAt(0);

            Assert.Equal(ApplyTo.Lock.ResourceOnly, activeLock.ApplyTo);
            Assert.Equal(LockScope.Shared, activeLock.LockScope);
            Assert.Equal("opaquelocktoken:15f67ec5-e229-4816-8cc7-d73f88d32220.c88101d1141db2de", activeLock.LockToken);
            Assert.Equal("Chuck Norris", activeLock.Owner.Value);
            Assert.Equal("http://example.com/1.txt", activeLock.LockRoot);
            Assert.Equal(20, activeLock.Timeout.Value.TotalSeconds);
        }

        [Fact]
        public void When_ResponseHasSeveralLocks_Should_ReturnThemAll()
        {
            var parser = new LockResponseParser();
            var response = parser.Parse(Responses.Lock.ResponseWithTwoLocks, 207, "Multi-Status");

            Assert.Equal(2, response.ActiveLocks.Count);
        }
    }
}
