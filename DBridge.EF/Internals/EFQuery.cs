using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace DBridge.EF.Internals
{
    internal class EFQuery<TModel> : IQuery<TModel>
    {
        private BridgeDbContext Db;
        private StandardFilter filter;
        private IndexSort[] sort;
        private int pageSize = 0;
        private int currentPage = 1;

        public EFQuery(BridgeDbContext db)
        {
            this.Db = db;
        }

        public IQuery<TModel> Filter(StandardFilter filter)
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

        public int Count()
        {
            IEnumerable<Record> records = GetRecords();
            return records.Count();
        }

        public IList<TModel> ToList()
        {
            IEnumerable<Record> records = GetRecords();

            var models = records.ToList()
                .Select(o => (TModel)o.GetModel())
                .ToList();
            return models;
        }

        private DbRawSqlQuery<Record> GetRecords()
        {
            var parameters = new List<object>();
            StringBuilder query = new StringBuilder();

            // SELECT, FROM and JOINs

            query.AppendLine(
@"SELECT Records.Id, Records.ClassName, Records.Storage, Records.Name
FROM  Records
LEFT JOIN Classes ON Classes.Name = Records.ClassName
LEFT JOIN Interfaces ON Interfaces.ClassName = Classes.Name"
            );

            if (sort != null)
            {
                int i = 0;
                foreach (var item in sort)
                {
                    query.AppendLine();
                    query.AppendFormat("LEFT JOIN (select * from FieldIndexes i where i.Name = '{0}') Sort{1} ON Sort{1}.RecordId = Records.Id",
                        item.IndexName, i);
                    i++;
                }
            }

            // WHERE

            query.AppendLine();
            query.Append("WHERE 1=1");

            if (typeof(TModel).IsInterface)
            {
                query.AppendLine();
                query.AppendFormat(@"AND Interfaces.Name = '{0}'", typeof(TModel).FullName);
            }
            else
            {
                query.AppendLine();
                query.AppendFormat(@"AND Records.ClassName = '{0}'", typeof(TModel).FullName);
            }

            if (filter != null)
            {
                var sqlWhere = new SqlWhere(filter);

                query.AppendLine();
                query.Append("AND (" + sqlWhere.Clause + ")");
                parameters.AddRange(sqlWhere.Parameters.Select((o, i) => new SqlParameter("@p" + i, o.Value)));
            }

            // GROUP BY

            query.AppendLine();
            query.Append("GROUP BY Records.Id, Records.ClassName, Records.Storage, Records.Name");

            // ORDER BY

            if (sort != null)
            {
                query.AppendLine();
                query.Append("ORDER BY ");
                query.Append(string.Join(", ", sort.Select((o, i) => 
                    (o.Ascending ? "max" : "min") + "(Sort" + i + ".Text)" + (o.Ascending ? " asc" : " desc") + ", " +
                    (o.Ascending ? "max" : "min") + "(Sort" + i + ".Moment)" + (o.Ascending ? " asc" : " desc") + ", " +
                    (o.Ascending ? "max" : "min") + "(Sort" + i + ".Number)" + (o.Ascending ? " asc" : " desc") + ", " +
                    (o.Ascending ? "max" : "min") + "(Sort" + i + ".Float)" + (o.Ascending ? " asc" : " desc")
                )));
            }
            else
            {
                query.AppendLine();
                query.Append("ORDER BY Records.Name");
            }

            // PAGING

            if (pageSize > 0)
            {
                query.AppendLine();
                query.Append(
@"OFFSET (@currentPage - 1) ROWS
FETCH NEXT @pageSize ROWS ONLY"
                );
                parameters.Add(new SqlParameter("currentPage", currentPage));
                parameters.Add(new SqlParameter("pageSize", pageSize));
            }

            var records = Db.Database.SqlQuery<Record>(query.ToString(), parameters.ToArray());

            return records;
        }
    }
}
