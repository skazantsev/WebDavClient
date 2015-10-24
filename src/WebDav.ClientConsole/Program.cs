using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebDav.ClientConsole
{
    public class Program
    {
        public static void Main()
        {
            TestWebDav().Wait();
            Console.ReadLine();
        }

        private static async Task TestWebDav()
        {
            using (var webDavClient = new WebDavClient(new WebDavClientParams { BaseAddress = new Uri("http://localhost:88/") }))
            {
                await webDavClient.Copy("1.txt", "_2.txt");

                await webDavClient.Move("http://localhost:88/_2.txt", "2.txt");

                await webDavClient.Mkcol("http://localhost:88/mydir");

                await webDavClient.PutFile("http://localhost:88/mydir/test.txt", File.OpenRead("test.txt"), "text/plain");

                await webDavClient.Move("mydir/test.txt", "http://localhost:88/mydir/test_ren.txt");

                await webDavClient.Copy("http://localhost:88/mydir/", "http://localhost:88/mydir1/");

                await webDavClient.Copy("http://localhost:88/mydir/", "http://localhost:88/mydir2/", new CopyParameters { ApplyTo = ApplyTo.Copy.ResourceOnly });

                var response = await webDavClient.GetRawFile("http://localhost:88/mydir/test_ren.txt");
                using (var reader = new StreamReader(response.Stream))
                {
                    var fileOutput = await reader.ReadToEndAsync();
                    Console.WriteLine(fileOutput);
                }

                await TestPropfind(webDavClient);

                await TestLock(webDavClient);

                await TestPropatch(webDavClient);

                await webDavClient.Delete("http://localhost:88/mydir");
            }
        }

        public static async Task TestPropfind(WebDavClient webDavClient)
        {
            var propfindParams = new PropfindParameters
            {
                CustomProperties = new XName[] {"testprop"},
                Namespaces = new[] {new NamespaceAttr("http://example.com")}
            };
            var response = await webDavClient.Propfind("http://localhost:88", propfindParams);
            Console.WriteLine(response.ToString());
            foreach (var res in response.Resources)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine("URI: {0}", res.Uri);
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
                Console.WriteLine("Properties: {0}", "[\r\n " + string.Join("\r\n ", res.Properties.Select(x => string.Format("{0}: {1}", x.Name, x.Value))) + "\r\n]");
                Console.WriteLine("PropertyStatuses: {0}", "[\r\n " + string.Join("\r\n ", res.PropertyStatuses.Select(x => x.ToString())) + "\r\n]");
                Console.WriteLine();
            }
        }

        public static async Task TestLock(WebDavClient webDavClient)
        {
            var response = await webDavClient.Lock("http://localhost:88/1.txt",
                    new LockParameters { LockScope = LockScope.Shared, Owner = new PrincipalLockOwner("Chuck Norris"), Timeout = TimeSpan.FromSeconds(120) });
            var token = string.Empty;
            foreach (var @lock in response.ActiveLocks)
            {
                token = @lock.LockToken;
                PrintActiveLock(@lock);
            }
            await webDavClient.Unlock("http://localhost:88/1.txt", token);
            Console.WriteLine("Unlocked!");

            var response2 = await webDavClient.Lock("http://localhost:88/2.txt");
            var token2 = response2.ActiveLocks.First().LockToken;
            var deleteResponse = await webDavClient.Delete("http://localhost:88/2.txt");
            if (!deleteResponse.IsSuccessful)
            {
                Console.WriteLine("Can't delete a resource. It's locked!");
            }

            deleteResponse = await webDavClient.Delete("http://localhost:88/2.txt", new DeleteParameters { LockToken = token2 });
            if (deleteResponse.IsSuccessful)
            {
                Console.WriteLine("The resource was deleted.");
            }
        }

        public static void PrintActiveLock(ActiveLock @lock)
        {
            Console.WriteLine(">>>LOCK");
            Console.WriteLine("LockRoot: {0}", @lock.LockRoot);
            Console.WriteLine("LockToken: {0}", @lock.LockToken);
            Console.WriteLine("LockScope: {0}", @lock.LockScope.HasValue ? Enum.GetName(typeof(LockScope), @lock.LockScope) : "null");
            Console.WriteLine("LockOwner: {0}", @lock.Owner != null ? @lock.Owner.Value : "null");
            Console.WriteLine("ApplyTo: {0}", Enum.GetName(typeof(ApplyTo.Lock), @lock.ApplyTo));
            Console.WriteLine("Timeout: {0}", @lock.Timeout.HasValue ? @lock.Timeout.Value.TotalSeconds.ToString() : "infinity");
            Console.WriteLine();
        }

        private static async Task TestPropatch(WebDavClient webDavClient)
        {
            var xns = "http://X_X";
            var @params = new ProppatchParameters
            {
                PropertiesToSet = new Dictionary<XName, string> { { "{DAV:}getcontenttype", "text/plain" }, { XName.Get("myprop", xns), "myval" } },
                PropertiesToRemove = new List<XName> { "{DAV:}ETag" },
                Namespaces = new List<NamespaceAttr> { new NamespaceAttr("x", xns) }
            };
            var response = await webDavClient.Proppatch("http://localhost:88/1.txt", @params);
            Console.WriteLine(response.ToString());
            Console.WriteLine("PropertyStatuses: {0}", "[\r\n " + string.Join("\r\n ", response.PropertyStatuses.Select(x => x.ToString())) + "\r\n]");
        }
    }
}
