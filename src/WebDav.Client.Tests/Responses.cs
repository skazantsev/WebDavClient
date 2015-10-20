namespace WebDav.Client.Tests
{
    public class Responses
    {
        public class Propfind
        {
            public static readonly string EmptyMultiStatusResponse = @"<D:multistatus xmlns:D=""DAV:""></D:multistatus>";

            public static readonly string ResponseWithoutProperties =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithOneFileProperty =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <testprop>test</testprop>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithWrongUriFormat =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>URI</D:href>
    <D:propstat>
      <D:prop>
        <testprop>test</testprop>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithWrongStatusFormat =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <testprop>test</testprop>
      </D:prop>
      <D:status>STATUS</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
        }
    }
}
