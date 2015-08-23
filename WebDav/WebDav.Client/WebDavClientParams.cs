using System.Collections.Generic;
using System.Net;

namespace WebDav
{
    public class WebDavClientParams
    {
        public WebDavClientParams()
        {
            AuthenticateAsCurrentUser = true;
            DefaultRequestHeaders = new Dictionary<string, string>();
            PreAuthenticate = true;
            UseProxy = true;
        }

        public bool AuthenticateAsCurrentUser { get; set; }

        public ICredentials Credentials { get; set; }

        public IDictionary<string, string> DefaultRequestHeaders { get; set; }

        public bool PreAuthenticate { get; set; }

        public IWebProxy Proxy { get; set; }

        public bool UseProxy { get; set; }
    }
}
