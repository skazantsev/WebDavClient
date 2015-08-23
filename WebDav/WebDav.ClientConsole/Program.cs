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
                Console.ReadLine();
            }
        }
    }
}
