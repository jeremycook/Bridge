using DataBridge.EF.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace DataBridge.EF
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
                    throw new InvalidOperationException("DataBridge.EF.EFBridge.ModelAssembies needs to be configured.");

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
            where TModel : class
        {
            var record = Db.Records.AsNoTracking().SingleOrDefault(o => o.Id == id);
            return (TModel)record.GetModel();
        }

        public IQuery<TModel> Query<TModel>()
        {
            return new EFQuery<TModel>(Db);
        }

        public void Insert(object model)
        {
            Type classType = model.GetType();
            UpsertClass(classType);

            Record record = new Record(model);
            Db.Records.Add(record);

            Db.SaveChanges();
        }

        public void InsertRange(IEnumerable<object> list)
        {
            list.Select(o => o.GetType()).Distinct().ToList()
                .ForEach(o => UpsertClass(o));

            Db.Records.AddRange(list.Select(o => new Record(o)));

            Db.SaveChanges();
        }

        public void Update(Guid id, object model)
        {
            Type classType = model.GetType();
            UpsertClass(classType);

            var record = Db.Records.SingleOrDefault(o => o.Id == id);
            record.SetModel(model);

            Db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var record = Db.Records.SingleOrDefault(o => o.Id == id);
            Db.Records.Remove(record);
            Db.SaveChanges();
        }

        public void DeleteRange(IEnumerable<Guid> recordIds)
        {
            var records = Db.Records.Where(o => recordIds.Contains(o.Id));
            Db.Records.RemoveRange(records);
            Db.SaveChanges();
        }


        public void Dispose()
        {
            Db.Dispose();
        }

        private void UpsertClass(Type classType)
        {
            var @class = Db.Classes.Include(o => o.Interfaces)
                .SingleOrDefault(o => o.Name == classType.FullName);
            if (@class == null)
            {
                @class = new Class(classType);
                Db.Classes.Add(@class);
            }
            else
            {
                @class.Update(classType);
            }
        }
    }
}
