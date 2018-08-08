using NSubstitute;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebDav.Infrastructure;

namespace WebDav.Client.Tests.TestDoubles
{
    public static class Dispatcher
    {
        internal static IWebDavDispatcher Mock(string content = "", int statusCode = 200, string description = "OK")
        {
            var dispatcher = Substitute.For<IWebDavDispatcher>();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(content),
                StatusCode = (HttpStatusCode)statusCode,
                ReasonPhrase = description
            };

            dispatcher
                .Send(Arg.Any<Uri>(), Arg.Any<HttpMethod>(), Arg.Any<RequestParameters>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromResult(response));
            dispatcher
                .Send(Arg.Any<Uri>(), Arg.Any<HttpMethod>(), Arg.Any<RequestParameters>(), Arg.Any<CancellationToken>(), Arg.Any<HttpCompletionOption>())
                .Returns(x => Task.FromResult(response));
            return dispatcher;
        }

        internal static IWebDavDispatcher MockFaulted()
        {
            var dispatcher = Substitute.For<IWebDavDispatcher>();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = (HttpStatusCode)500,
                ReasonPhrase = "Internal Server Error"
            };

            dispatcher
                .Send(Arg.Any<Uri>(), Arg.Any<HttpMethod>(), Arg.Any<RequestParameters>(), Arg.Any<CancellationToken>())
                .Returns(x => Task.FromResult(response));
            dispatcher
                .Send(Arg.Any<Uri>(), Arg.Any<HttpMethod>(), Arg.Any<RequestParameters>(), Arg.Any<CancellationToken>(), Arg.Any<HttpCompletionOption>())
                .Returns(x => Task.FromResult(response));
            return dispatcher;
        }
    }
}
