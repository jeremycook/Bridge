using DataBridge.Db.Internal;
using DataBridge.Db.Meta;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DataBridge.Db
{
    public class DbBridge : IBridge, IDisposable
    {
        /// <summary>
        /// Set the assemblies that should be searched for model types.
        /// </summary>
        public static IEnumerable<Assembly> ModelAssemblies
        {
            get
            {
                if (_ModelAssemblies == null)
                    throw new InvalidOperationException("DataBridge.Db.DbBridge.ModelAssembies needs to be configured.");

                return _ModelAssemblies;
            }
            set { _ModelAssemblies = value; }
        }
        private static IEnumerable<Assembly> _ModelAssemblies;

        private readonly IDbConnection Db;

        public DbBridge(IDbConnection db)
        {
            Db = db;
            if (Db.State != ConnectionState.Open)
            {
                Db.Open();
            }
        }

        public TModel Get<TModel>(Guid id)
            where TModel : class
        {
            Record record = Db.Query<Record>("select * from dbo.Records where Id = @id", new { id }).SingleOrDefault();
            return (TModel)record.GetModel();
        }

        public IQuery<TModel> Query<TModel>()
        {
            return new DbQuery<TModel>(Db);
        }

        public void Insert(object model)
        {
            InsertRange(new[] { model });
        }

        public void InsertRange(IEnumerable<object> list)
        {
            using (var tx = Db.BeginTransaction())
            {
                list.Select(o => o.GetType()).Distinct().ToList()
                    .ForEach(o => UpsertClass(o, tx));

                var records = list.Select(o => new Record(o)).ToList();

                Db.Execute("insert into dbo.Records (Id, Classname, Name, Storage) values (@id, @className, @name, @storage)",
                    records.Select(record => new { record.Id, record.ClassName, record.Name, record.Storage }),
                    tx
                );

                // Insert field indexes.
                Db.Execute("insert into dbo.FieldIndexes (RecordId, Name, Guid, Text, Moment, Number, Float) values (@RecordId, @Name, @Guid, @Text, @Moment, @Number, @Float)",
                    records.SelectMany(o => o.FieldIndexes).Select(record => new { record.RecordId, record.Name, record.Guid, record.Text, record.Moment, record.Number, record.Float }),
                    tx
                );

                tx.Commit();
            }
        }

        public void Update(Guid id, object model)
        {
            //Type classType = model.GetType();
            // TODO? UpsertClass(classType);

            Record record = Db.Query<Record>("select * from dbo.Records where Id = @Id", new { Id = id }).Single();
            record.FieldIndexes = Db.Query<FieldIndex>("select * from dbo.FieldIndexes where RecordId = @RecordId", new { RecordId = id }).ToList();

            if (record.ClassName != model.GetType().FullName)
            {
                throw new InvalidOperationException(string.Format("The type of `model` changed from '{0}' to '{1}'.",
                    record.ClassName, model.GetType().FullName));
            }

            record.SetModel(model);

            Db.Execute("update dbo.Records set Name = @name, Storage = @storage from dbo.Records where Id = @id",
                new { record.Id, record.Name, record.Storage });
        }

        public void Delete(Guid id)
        {
            DeleteRange(new[] { id });
        }

        public void DeleteRange(IEnumerable<Guid> recordIds)
        {
            Db.Execute("delete dbo.Records where Id in @ids", new { Ids = recordIds });
        }


        public void Dispose()
        {
        }

        private void UpsertClass(Type classType, IDbTransaction tx)
        {
            var count = Db.ExecuteScalar<int>("select count(*) from Classes where Name = @name", new { Name = classType.FullName }, tx);
            if (count == 0)
            {
                Db.Execute("insert into dbo.Classes(Name) values (@name)", new { Name = classType.FullName }, tx);
                Db.Execute("insert into dbo.Interfaces(ClassName, Name) values (@className, @name)",
                    classType.GetInterfaces().Select(o => new { ClassName = classType.FullName, Name = o.FullName }),
                    tx
                );
            }
            else
            {
                // TODO? Update a classes interfaces. Not sure if this is really needed.
                //@class.Update(classType);
            }
        }
    }
}
