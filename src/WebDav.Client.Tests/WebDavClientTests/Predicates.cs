using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebDav.Client.Tests.WebDavClientTests
{
    internal static class Predicates
    {
        public static Expression<Predicate<RequestParameters>> CompareDepthHeader(string expectedDepthHeaderValue)
        {
            return x => x.Headers.Any(h => h.Key == "Depth" && h.Value == expectedDepthHeaderValue);
        }

        public static Expression<Predicate<RequestParameters>> CompareRequestContent(string expectedContent)
        {
            return x => expectedContent == x.Content.ReadAsStringAsync().Result;
        }
    }
}
