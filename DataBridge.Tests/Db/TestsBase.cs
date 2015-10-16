using Dapper;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBridge.Tests.Models;
using Foundation.ObjectHydrator;
using DataBridge.Db;
using System.Data.SqlClient;
using System.Linq;

namespace DataBridge.Tests.Db
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
            (Bridge as IDisposable).Dispose();

            Connection.Execute("delete from dbo.Records");
            (Connection as IDisposable).Dispose();
        }
    }
}
