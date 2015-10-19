namespace WebDav.Response
{
    internal interface IResponseParser<out TResponse>
    {
        TResponse Parse(string response, int statusCode, string description);
    }
}
