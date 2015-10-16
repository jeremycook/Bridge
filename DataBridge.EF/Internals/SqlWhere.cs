using System.Collections.Generic;
using System.Linq;

namespace DataBridge.EF.Internals
{
    internal class SqlWhere
    {
        public SqlWhere(StandardFilter filter)
        {
            Clause = Apply("{0}", filter);
            Parameters = new List<Literal>();
        }

        public string Clause { get; protected set; }
        public List<Literal> Parameters { get; protected set; }

        internal string Apply(string format, params StandardFilter[] filters)
        {
            var args = new List<object>();
            foreach (var filter in filters)
            {
                if (filter is And)
                {
                    args.Add(Apply("({0}) and ({1})", (filter as And).LeftFilter, (filter as And).RightFilter));
                    continue;
                }

                if (filter is Or)
                {
                    args.Add(Apply("({0}) or ({1})", (filter as Or).LeftFilter, (filter as Or).RightFilter));
                    continue;
                }

                if (filter is Not)
                {
                    args.Add(Apply("not ({0})", (filter as Not).Filter));
                    continue;
                }

                if (filter is Eq)
                {
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] {2})",
                        (filter as Eq).Field.Name,
                        (filter as Eq).Literal.ValueType,
                        (filter as Eq).Literal.Value == null ? "is null" : "= @p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Eq).Literal);
                    continue;
                }

                if (filter is Lt)
                {
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] < {2})",
                        (filter as Lt).Field.Name,
                        (filter as Lt).Literal.ValueType,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Lt).Literal);
                    continue;
                }

                if (filter is Lte)
                {
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] <= {2})",
                        (filter as Lte).Field.Name,
                        (filter as Lte).Literal.ValueType,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Lte).Literal);
                    continue;
                }

                if (filter is Gt)
                {
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] > {2})",
                        (filter as Gt).Field.Name,
                        (filter as Gt).Literal.ValueType,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Gt).Literal);
                    continue;
                }

                if (filter is Gte)
                {
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] >= {2})",
                        (filter as Gte).Field.Name,
                        (filter as Gte).Literal.ValueType,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Gte).Literal);
                    continue;
                }

                if (filter is In)
                {
                    var inf = filter as In;
                    args.Add(string.Format(
                        "exists (select * from FieldIndexes where FieldIndexes.RecordId = Records.Id and FieldIndexes.[Name] = '{0}' and FieldIndexes.[{1}] in ({2}))",
                        inf.Field.Name,
                        inf.Literals[0].ValueType,
                        string.Join(", ", Enumerable.Range(0, inf.Literals.Count()).Select(o => "@p" + (Parameters.Count + o)))
                    ));
                    Parameters.AddRange(inf.Literals);
                    continue;
                }
            }

            return string.Format(format, args.ToArray());
        }
    }
}