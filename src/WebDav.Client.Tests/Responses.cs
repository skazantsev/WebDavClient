namespace WebDav.Client.Tests
{
    public class Responses
    {
        public class Propfind
        {
            public static readonly string EmptyPropResponse =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:propfind xmlns:D=""DAV:"">
    <D:prop xmlns:R=""http://ns.example.com/boxschema/""></D:prop>
</D:propfind>";
        }
    }
}
