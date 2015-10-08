using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bridge.Tests.Models;
using Foundation.ObjectHydrator;
using Bridge.Db;
using System.Data.SqlClient;
using System.Linq;

namespace Bridge.Tests.Db
{
    [TestClass]
    public class TestsBase : IDisposable
    {
        public static readonly DateTimeOffset Created = DateTimeOffset.Now;
        public static readonly Guid CreatedById = Guid.Parse("c1fa075a-be6e-4f60-b62f-02001dfb38f3");

        public static Hydrator<Post> PostHydrador = new Hydrator<Post>();
        public static Hydrator<Link> LinkHydrador = new Hydrator<Link>();

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            DbBridge.ModelAssemblies = new[] { typeof(Post).Assembly };
        }


        private readonly SqlConnection Connection;
        public readonly IBridge Bridge;

        public TestsBase()
        {
            Connection = new SqlConnection(@"Data Source=.\sqlexpress; Initial Catalog=EFBridgeTests; Integrated Security=True");
            Bridge = new DbBridge(Connection);
        }


        public void Dispose()
        {
            Bridge.DeleteRange(Bridge.Query<IIdentify>().ToList().Select(o => o.Id));
            (Bridge as IDisposable).Dispose();
            (Connection as IDisposable).Dispose();
        }
    }
}
