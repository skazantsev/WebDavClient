# WebDAV .NET client [![Build status](https://ci.appveyor.com/api/projects/status/xee0yxvah59ffvd3?svg=true)](https://ci.appveyor.com/project/skazantsev/webdavclient)

Asynchronous cross-platform WebDAV client for .NET Standard 1.1+. It aims to have a full support of [RFC4918](http://www.webdav.org/specs/rfc4918.html).

## Installation
Install WebDav.Client via [NuGet](https://www.nuget.org/packages/WebDav.Client/).
```
Install-Package WebDav.Client
```

## Supported platforms
- .NET Framework 4.5+
- .NET Core
- Mono
- Xamarin
- UWP

For more information see [.NET Standard](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

## Usage examples

**Basic usage:**
``` csharp
using (var webDavClient = new WebDavClient())
{
    var result = await webDavClient.Propfind("http://mywebdav/1.txt");
    if (result.IsSuccessful)
        // continue ...
    else
        // handle an error
}
```

**Using BaseAddress:**
``` csharp
var clientParams = new WebDavClientParams { BaseAddress = new Uri("http://mywebdav/") };
using (var webDavClient = new WebDavClient(clientParams))
{
    await webDavClient.Propfind("1.txt");
}
```

**Operations with files and directories (resources & collections):**
``` csharp
var clientParams = new WebDavClientParams { BaseAddress = new Uri("http://mywebdav/") };
using (var webDavClient = new WebDavClient(clientParams))
{
    await webDavClient.Mkcol("mydir"); // create a directory

    await webDavClient.Copy("source.txt", "dest.txt"); // copy a file

    await webDavClient.Move("source.txt", "dest.txt"); // move a file

    await webDavClient.Delete("file.txt", "dest.txt"); // delete a file

    await webDavClient.GetRawFile("file.txt"); // get a file without processing from the server

    await webDavClient.GetProcessedFile("file.txt"); // get a file that can be processed by the server

    await webDavClient.PutFile("file.xml", File.OpenRead("file.xml"), "text/xml"); // upload a resource
}
```

**PROPFIND example:**
``` csharp
// list files & subdirectories in 'mydir'
var result = await webDavClient.Propfind("http://mywebdav/mydir");
if (result.IsSuccessful)
{
    foreach (var res in result.Resources)
    {
        Trace.WriteLine("Name: " + res.DisplayName);
        Trace.WriteLine("Is directory: " + res.IsCollection);
        // other params
    }
}
```

**Authentication example:**
``` csharp
var clientParams = new WebDavClientParams
{
    BaseAddress = new Uri("http://mywebdav/"),
    Credentials = new NetworkCredential("user", "12345")
};
using (var webDavClient = new WebDavClient(clientParams))
{
    // call webdav methods...
}
```

**Synchronous API:**
``` csharp
  // will block the current thread, so use it cautiously
  var result = webDavClient.Propfind("1.txt").Result;
```

## License
WebDAVClient is licensed under the MIT License. See [LICENSE.txt](https://github.com/skazantsev/WebDavClient/blob/master/LICENSE.txt) for more details.
