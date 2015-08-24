using System.Net.Http;

namespace WebDav.Helpers
{
    internal sealed class WebDavMethod
    {
        public static readonly HttpMethod Propfind = new HttpMethod("PROPFIND");

        public static readonly HttpMethod Proppatch = new HttpMethod("PROPPATCH");

        public static readonly HttpMethod Mkcol = new HttpMethod("MKCOL");

        public static readonly HttpMethod Copy = new HttpMethod("COPY");

        public static readonly HttpMethod Move = new HttpMethod("MOVE");
    }
}
