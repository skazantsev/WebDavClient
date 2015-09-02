using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebDav.ClientConsole
{
    public class Program
    {
        public static void Main()
        {
            TestWebDav().Wait();
        }

        private static async Task TestWebDav()
        {
            using (var webDavClient = new WebDavClient())
            {
                await webDavClient.Copy("http://localhost:88/1.txt", "http://localhost:88/_2.txt");

                await webDavClient.Move("http://localhost:88/_2.txt", "http://localhost:88/2.txt");

                await webDavClient.Mkcol("http://localhost:88/mydir");

                await webDavClient.PutFile("http://localhost:88/mydir/test.txt", File.OpenRead("test.txt"), "text/plain");

                await webDavClient.Move("http://localhost:88/mydir/test.txt", "http://localhost:88/mydir/test_ren.txt");

                await webDavClient.Copy("http://localhost:88/mydir/", "http://localhost:88/mydir1/");

                await webDavClient.Copy("http://localhost:88/mydir/", "http://localhost:88/mydir2/", ApplyTo.Copy.CollectionOnly);

                var fileStream = await webDavClient.GetFile("http://localhost:88/mydir/test_ren.txt");
                using (var reader = new StreamReader(fileStream))
                {
                    var fileOutput = await reader.ReadToEndAsync();
                    Console.WriteLine(fileOutput);
                }

                var response = await webDavClient.Propfind("http://localhost:88", new [] {"testprop"});
                foreach (var res in response.Resources)
                {
                    Console.WriteLine("====================================================");
                    Console.WriteLine("HREF: {0}", res.Href);
                    Console.WriteLine("====================================================");
                    Console.WriteLine("IsCollection: {0}", res.IsCollection);
                    Console.WriteLine("IsHidden: {0}", res.IsHidden);
                    Console.WriteLine("CreationDate: {0}", res.CreationDate);
                    Console.WriteLine("DisplayName: {0}", res.DisplayName);
                    Console.WriteLine("ContentLanguage: {0}", res.ContentLanguage);
                    Console.WriteLine("ContentLength: {0}", res.ContentLength);
                    Console.WriteLine("ContentType: {0}", res.ContentType);
                    Console.WriteLine("ETag: {0}", res.ETag);
                    Console.WriteLine("LastModifiedDate: {0}", res.LastModifiedDate);
                    Console.WriteLine("Properties: {0}", "[\r\n -" + string.Join("\r\n -", res.Properties.Select(kv => string.Format("{0}: {1}", kv.Key, kv.Value))) + "\r\n]");
                    Console.WriteLine();
                }

                // await webDavClient.Proppatch("http://localhost:88/1.txt", new Dictionary<string, string> { { "DisplayName", "111" } }, new List<string> { "ETag" });

                Console.ReadLine();
            }
        }
    }
}
