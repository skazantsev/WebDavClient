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

            public static readonly string FileResponseWithOneProperty =
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

            public static readonly string ResponseWithIsCollectionZeroProperty =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/folder</D:href>
    <D:propstat>
      <D:prop>
        <D:iscollection>0</D:iscollection>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithIsCollectionOneProperty =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/folder</D:href>
    <D:propstat>
      <D:prop>
        <D:iscollection>1</D:iscollection>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithResourceTypeCollectionProperty =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/folder</D:href>
    <D:propstat>
      <D:prop>
        <D:resourcetype><D:collection/></D:resourcetype>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string FileResponse403Status =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <testprop>test</testprop>
      </D:prop>
      <D:status>HTTP/1.1 403 Forbidden</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string FileResponseWithStandardProperties =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/1.txt</D:href>
    <D:propstat>
      <D:status>HTTP/1.1 200 OK</D:status>
      <D:prop>
        <D:getcontenttype>text/plain</D:getcontenttype>
        <D:getlastmodified>Thu, 03 Sep 2015 20:49:19 GMT</D:getlastmodified>
        <D:ishidden>0</D:ishidden>
        <D:supportedlock>
          <D:lockentry>
            <D:lockscope>
              <D:exclusive/>
            </D:lockscope>
            <D:locktype>
              <D:write/>
            </D:locktype>
          </D:lockentry>
          <D:lockentry>
            <D:lockscope>
              <D:shared/>
            </D:lockscope>
            <D:locktype>
              <D:write/>
            </D:locktype>
          </D:lockentry>
        </D:supportedlock>
        <D:lockdiscovery>
          <D:activelock>
            <D:locktype><D:write/></D:locktype>
            <D:lockscope><D:exclusive/></D:lockscope>
            <D:depth>0</D:depth>
            <D:owner>@Me</D:owner>
            <D:timeout>Infinite</D:timeout>
            <D:locktoken>
              <D:href>urn:uuid:f81de2ad-7f3d-a1b2-4f3c-00a0c91a9d76</D:href>
            </D:locktoken>
            <D:lockroot> 
              <D:href>http://www.example.com/1.txt</D:href> 
            </D:lockroot>
          </D:activelock>
        </D:lockdiscovery>
        <D:getetag>""c5f02718ae6d01:0""</D:getetag>
        <D:displayname>1.txt</D:displayname>
        <D:getcontentlanguage>en</D:getcontentlanguage>
        <D:getcontentlength>4</D:getcontentlength>
        <D:iscollection>0</D:iscollection>
        <D:creationdate>2015-08-19T22:59:25.16Z</D:creationdate>
        <D:resourcetype/>
      </D:prop>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithComplexProperties =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <author>
          <Name>J.J. Johnson</Name>
        </author>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string ResponseWithTwoResources =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<multistatus xmlns=""DAV:"">
  <response>
    <href>http://www.example.com/container/</href>
    <propstat>
      <prop>
        <author/>
      </prop>
      <status>HTTP/1.1 200 OK</status>
    </propstat>
  </response>
  <response>
    <href>http://www.example.com/container/front.html</href>
    <propstat>
      <prop>
        <displayname/>
      </prop>
      <status>HTTP/1.1 403 Forbidden</status>
    </propstat>
  </response>
</multistatus>";

            public static readonly string ResponseWithPropertyWithXmlNamespace =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop xmlns:R=""http://ns.example.com/boxschema/"">
        <R:author>
          <R:Name>J.J. Johnson</R:Name>
        </R:author>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";

            public static readonly string FileResponseWithMultipleSimilarProperties =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <testprop>v1</testprop>
        <testprop>v2</testprop>
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

            public static readonly string ResponseWithoutXmlDeclaration =
                @"<D:multistatus xmlns:D=""DAV:"">
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

            public static readonly string ResponseWithIncorectCreationDateFormat =
                @"<D:multistatus xmlns:D=""DAV:"">
  <D:response>
    <D:href>http://www.example.com/file</D:href>
    <D:propstat>
      <D:prop>
        <D:creationdate>Novemer 24, 2015</D:creationdate>
      </D:prop>
      <D:status>HTTP/1.1 200 OK</D:status>
    </D:propstat>
  </D:response>
</D:multistatus>";
        }
    }
}
