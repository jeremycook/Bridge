using Bridge.EF.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bridge.EF
{
    public class EFBridge : IBridge, IDisposable
    {
        /// <summary>
        /// Set the assemblies that should be searched for model types.
        /// </summary>
        public static IEnumerable<Assembly> ModelAssemblies
        {
            get
            {
                if (_ModelAssemblies == null)
                    throw new InvalidOperationException("Bridge.EF.EFBridge.ModelAssembies needs to be configured.");

                return _ModelAssemblies;
            }
            set { _ModelAssemblies = value; }
        }
        private static IEnumerable<Assembly> _ModelAssemblies;

        private readonly BridgeDbContext Db;

        public EFBridge(string nameOrConnectionString)
        {
            Db = new BridgeDbContext(nameOrConnectionString);
        }

        public TModel Get<TModel>(Guid id)
        {
            var record = Db.Records.AsNoTracking().SingleOrDefault(o => o.Id == id);
            return (TModel)record.GetModel();
        }

        public IQuery<TModel> Query<TModel>()
        {
            return new EFQuery<TModel>(Db);
        }

        public void Insert<TModel>(TModel model)
        {
            Record record = new Record(model);
            Db.Records.Add(record);
            Db.SaveChanges();
        }

        public void InsertRange<TModel>(IEnumerable<TModel> list)
        {
            Db.Records.AddRange(list.Select(o => new Record(o)));
            Db.SaveChanges();
        }

        public void Update<TModel>(Guid id, TModel model)
        {
            var record = Db.Records.SingleOrDefault(o => o.Id == id);
            record.SetModel(model);
            Db.SaveChanges();
        }

        public void Delete<TModel>(Guid id)
        {
            var record = Db.Records.SingleOrDefault(o => o.Id == id);
            Db.Records.Remove(record);
            Db.SaveChanges();
        }

        public void DeleteRange<TModel>(IEnumerable<Guid> recordIds)
        {
            var records = Db.Records.Where(o => recordIds.Contains(o.Id));
            Db.Records.RemoveRange(records);
            Db.SaveChanges();
        }

        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
