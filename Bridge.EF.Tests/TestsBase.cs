using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bridge.Tests.Models;
using Foundation.ObjectHydrator;

namespace Bridge.EF.Tests
{
    [TestClass]
    public class TestsBase : IDisposable
    {
        public readonly IBridge Bridge = new EFBridge(@"Data Source=.\sqlexpress; Initial Catalog=EFBridgeTests; Integrated Security=True");

        public static readonly DateTimeOffset Created = DateTimeOffset.Now;
        public static readonly Guid CreatedById = Guid.Parse("c1fa075a-be6e-4f60-b62f-02001dfb38f3");

        public static Hydrator<Post> PostHydrador = new Hydrator<Post>();
        public static Hydrator<Link> LinkHydrador = new Hydrator<Link>();

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            EFBridge.ModelAssemblies = new[] { typeof(Post).Assembly };
        }

        public void Dispose()
        {
            (Bridge as IDisposable).Dispose();
        }
    }
}
