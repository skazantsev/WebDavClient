using System;
using System.Linq;
using System.Xml.Linq;

namespace WebDav.Response
{
    internal class ProppatchResponseParser : IResponseParser<ProppatchResponse>
    {
        public ProppatchResponse Parse(string response, int statusCode, string description)
        {
            if (string.IsNullOrEmpty(response))
                return new ProppatchResponse(statusCode, description);

            var xresponse = XDocument.Parse(response);
            if (xresponse.Root == null)
                return new ProppatchResponse(statusCode, description);

            var propStatuses = xresponse.Root.LocalNameElements("response", StringComparison.OrdinalIgnoreCase)
                .SelectMany(MultiStatusParser.GetPropertyStatuses)
                .ToList();
            return new ProppatchResponse(statusCode, description, propStatuses);
        }
    }
}
