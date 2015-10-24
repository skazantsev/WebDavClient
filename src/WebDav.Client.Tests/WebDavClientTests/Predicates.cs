using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebDav.Client.Tests.WebDavClientTests
{
    internal static class Predicates
    {
        public static Expression<Predicate<RequestParameters>> CompareHeader(string headerName, string headerValue)
        {
            return x => x.Headers.Any(h => h.Key == headerName && h.Value == headerValue);
        }

        public static Expression<Predicate<RequestParameters>> CompareRequestContent(string expectedContent)
        {
            return x => expectedContent == x.Content.ReadAsStringAsync().Result;
        }
    }
}
