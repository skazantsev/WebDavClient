using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("WebDav.Client")]
[assembly: AssemblyCompany("skazantsev")]
[assembly: AssemblyProduct("WebDav.Client")]
[assembly: AssemblyDescription("An easy-to-use async WebDAV client for .NET")]
[assembly: AssemblyCopyright("Copyright © 2016 skazantsev")]
[assembly: ComVisible(false)]
[assembly: Guid("0cc4b1b7-57f0-456a-8512-b8d69e34a550")]
[assembly: AssemblyVersion("1.0.4.0")]
[assembly: AssemblyFileVersion("1.0.4.0")]

#if SIGNED
[assembly: InternalsVisibleTo("WebDav.Client.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f130ca0ab17e20a726c71d1629f62002952816a0e1c5dc255118b348a8137f0e9c4b96f448d680a6fc72b30b1c28b7d8b19dca5c812f76230c51c8247e3ee687725635ec749a1f32bcf540913d9bd04d40cd834e9c1103cc4ec47669a0f6348a35fd11d1f935a39d8bfe9721e66f9996ccfa7e88cf38e1806e9cbc1b7256c9b6")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
#else
[assembly: InternalsVisibleTo("WebDav.Client.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
