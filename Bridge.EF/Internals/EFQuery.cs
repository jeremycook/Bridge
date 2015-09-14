using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using LinqKit;

namespace Bridge.EF.Internals
{
    internal class EFQuery<TModel> : IQuery<TModel>
    {
        private BridgeDbContext Db;
        private Expression<Func<IIndex, bool>> filter;
        private IndexSort[] sort;
        private int pageSize = 0;
        private int currentPage = 1;

        public EFQuery(BridgeDbContext db)
        {
            this.Db = db;
        }

        public IQuery<TModel> Filter(Expression<Func<IIndex, bool>> filter)
        {
            this.filter = filter;
            return this;
        }

        public IQuery<TModel> Sort(params IndexSort[] sort)
        {
            this.sort = sort;
            return this;
        }

        public IQuery<TModel> Page(int pageSize, int currentPage = 1)
        {
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            return this;
        }

        public IList<TModel> ToList()
        {
            IQueryable<Record> records = GetRecords();

            var models = records.ToList()
                .Select(o => (TModel)o.GetModel())
                .ToList();
            return models;
        }

        private IQueryable<Record> GetRecords()
        {
            IQueryable<Record> records = Db.Records.AsNoTracking()
                .AsExpandable()
                .Where(o => o.TypeName == typeof(TModel).FullName)
                .OrderBy(o => o.Name);

            if (filter != null)
            {
                records = records.Where(o => o.Indices.Any(i => filter.Invoke(i)));
            }

            if (sort != null)
            {
                foreach (var item in sort)
                {
                    records = ApplySort(records as IOrderedQueryable<Record>, item);
                }
            }

            if (pageSize > 0)
            {
                records = records.Skip((currentPage - 1) * pageSize).Take(pageSize);
            }

            return records;
        }

        private static IQueryable<Record> ApplySort(IOrderedQueryable<Record> records, IndexSort sortOn)
        {
            if (sortOn.Ascending)
            {
                records = records.ThenBy(o =>
                    o.Indices
                        .Where(i => i.Name.Equals(sortOn.IndexName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderBy(i => i.Value)
                        .Select(i => i.Value)
                        .FirstOrDefault()
                );
            }
            else
            {
                records = records.ThenByDescending(o =>
                    o.Indices
                        .Where(i => i.Name.Equals(sortOn.IndexName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(i => i.Value)
                        .Select(i => i.Value)
                        .FirstOrDefault()
                );
            }
            return records;
        }
    }
}
