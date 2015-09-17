using System;
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

                await webDavClient.Copy("http://localhost:88/mydir/", "http://localhost:88/mydir2/", new CopyParameters { ApplyTo = ApplyTo.Copy.ResourceOnly });

                var fileStream = await webDavClient.GetRawFile("http://localhost:88/mydir/test_ren.txt");
                using (var reader = new StreamReader(fileStream))
                {
                    var fileOutput = await reader.ReadToEndAsync();
                    Console.WriteLine(fileOutput);
                }

                await TestPropfind(webDavClient);

                //await webDavClient.Proppatch("http://localhost:88/1.txt", new Dictionary<string, string> { { "DisplayName", "111" } }, new List<string> { "ETag" });

                await TestLock(webDavClient);

                await webDavClient.Delete("http://localhost:88/mydir");

                Console.ReadLine();
            }
        }

        public static async Task TestPropfind(WebDavClient webDavClient)
        {
            var response = await webDavClient.Propfind("http://localhost:88", new[] { "testprop" });
            foreach (var res in response.Resources)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine("HREF: {0}", res.Href);
                Console.WriteLine("====================================================");
                foreach (var @lock in res.ActiveLocks)
                {
                    PrintActiveLock(@lock);
                }
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
        }

        public static async Task TestLock(WebDavClient webDavClient)
        {
            var activeLocks = await webDavClient.Lock("http://localhost:88/1.txt",
                    new LockParameters { LockScope = LockScope.Shared, Owner = new PrincipalLockOwner("Chuck Norris"), Timeout = TimeSpan.FromSeconds(120) });
            var token = string.Empty;
            foreach (var @lock in activeLocks)
            {
                token = @lock.LockToken;
                PrintActiveLock(@lock);
            }
            await webDavClient.Unlock("http://localhost:88/1.txt", token);
            Console.WriteLine("Unlocked!");

            var activeLocks2 = await webDavClient.Lock("http://localhost:88/2.txt");
            var token2 = activeLocks2.First().LockToken;
            try
            {
                await webDavClient.Delete("http://localhost:88/2.txt");
            }
            catch
            {
                Console.WriteLine("Can't delete a resource. It's locked!");
            }

            await webDavClient.Delete("http://localhost:88/2.txt", new DeleteParameters { LockToken = token2 });
            Console.WriteLine("The resource was deleted.");
        }

        public static void PrintActiveLock(ActiveLock @lock)
        {
            Console.WriteLine(">>>LOCK");
            Console.WriteLine("HREF: {0}", @lock.ResourceHref);
            Console.WriteLine("LockToken: {0}", @lock.LockToken);
            Console.WriteLine("LockScope: {0}", @lock.LockScope.HasValue ? Enum.GetName(typeof(LockScope), @lock.LockScope) : "null");
            Console.WriteLine("LockOwner: {0}", @lock.Owner != null ? @lock.Owner.Value : "null");
            Console.WriteLine("ApplyTo: {0}", Enum.GetName(typeof(ApplyTo.Lock), @lock.ApplyTo));
            Console.WriteLine("Timeout: {0}", @lock.Timeout.HasValue ? @lock.Timeout.Value.TotalSeconds.ToString() : "infinity");
            Console.WriteLine();
        }
    }
}
