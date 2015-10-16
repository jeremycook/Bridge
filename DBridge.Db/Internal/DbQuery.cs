using DBridge.Db.Meta;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBridge.Db.Internal
{
    internal class DbQuery<TModel> : IQuery<TModel>
    {
        private readonly IDbConnection db;

        private StandardFilter filter;
        private IndexSort[] sort;
        private int pageSize = 0;
        private int currentPage = 1;

        private CommandDefinition? _CommandDefinition;

        public DbQuery(IDbConnection db)
        {
            this.db = db;
        }

        public IQuery<TModel> Filter(StandardFilter filter)
        {
            this.filter = filter;
            return this;
        }

        public IQuery<TModel> Page(int pageSize, int currentPage = 1)
        {
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            return this;
        }

        public IQuery<TModel> Sort(params IndexSort[] indexSorts)
        {
            this.sort = indexSorts;
            return this;
        }

        public int Count()
        {
            var cd = GetCommandDefinition();
            string query = string.Format("SELECT count(*) FROM ({0}) data", cd.CommandText);

            int count = db.ExecuteScalar<int>(query, cd.Parameters);
            return count;
        }

        public IList<TModel> ToList()
        {
            var cd = GetCommandDefinition();
            var records = db.Query<Record>(cd.CommandText, cd.Parameters);
            var models = records.Select(o => (TModel)o.GetModel()).ToList();
            return models;
        }


        private CommandDefinition GetCommandDefinition()
        {
            if (_CommandDefinition == null)
            {
                var query = new StringBuilder();
                var parameters = new List<KeyValuePair<string, object>>();

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
                    parameters.AddRange(sqlWhere.Parameters.Select((o, i) => new KeyValuePair<string, object>("p" + i, o.Value)));
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
                else if (pageSize > 0)
                {
                    // ORDER BY is required when paging.
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
                    parameters.Add(new KeyValuePair<string, object>("currentPage", currentPage));
                    parameters.Add(new KeyValuePair<string, object>("pageSize", pageSize));
                }

                _CommandDefinition = new CommandDefinition(query.ToString(), new DynamicParameters(parameters));
            }

            return _CommandDefinition.Value;
        }
    }
}