using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBridge.EF.Internals
{
    internal class BridgeDbContext : DbContext
    {
        static BridgeDbContext()
        {
            Database.SetInitializer<BridgeDbContext>(new NullDatabaseInitializer<BridgeDbContext>());
        }

        [Obsolete("Runtime only", true)]
        public BridgeDbContext() { }
        public BridgeDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString: nameOrConnectionString)
        { }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
