using System;
using System.IO;
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

                var fileStream = await webDavClient.GetFile("http://localhost:88/mydir/test_ren.txt");
                using (var reader = new StreamReader(fileStream))
                {
                    var fileOutput = await reader.ReadToEndAsync();
                    Console.WriteLine(fileOutput);
                }

                var response = await webDavClient.Propfind("http://localhost:88");
                foreach (var res in response.Resources)
                {
                    Console.WriteLine("HREF: {0}", res.Href);
                    Console.WriteLine("====================================================");
                    Console.WriteLine("IsCollection: {0}", res.IsCollection);
                    Console.WriteLine("CreationDate: {0}", res.CreationDate);
                    Console.WriteLine("DisplayName: {0}", res.DisplayName);
                    Console.WriteLine("ContentLanguage: {0}", res.ContentLanguage);
                    Console.WriteLine("ContentLength: {0}", res.ContentLength);
                    Console.WriteLine("ContentType: {0}", res.ContentType);
                    Console.WriteLine("ETag: {0}", res.ETag);
                    Console.WriteLine("LastModifiedDate: {0}", res.LastModifiedDate);
                    Console.WriteLine();
                }

                Console.ReadLine();
            }
        }
    }
}
