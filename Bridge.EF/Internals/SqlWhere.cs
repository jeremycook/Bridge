using System.Collections.Generic;
using System.Linq;

namespace Bridge.EF.Internals
{
    internal class SqlWhere
    {
        public SqlWhere(StandardFilter filter)
        {
            Clause = Apply("{0}", filter);
        }

        public string Clause { get; protected set; }
        public List<Literal> Parameters { get; protected set; } = new List<Literal>();

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
                        "Indices.[Name] = '{0}' and Indices.[Value] {1}",
                        (filter as Eq).Field.Name,
                        (filter as Eq).Literal.Value == null ? "is null" : "= @p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Eq).Literal);
                    continue;
                }

                if (filter is Lt)
                {
                    args.Add(string.Format(
                        "Indices.[Name] = '{0}' and Indices.[Value] < {1}",
                        (filter as Lt).Field.Name,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Lt).Literal);
                    continue;
                }

                if (filter is Lte)
                {
                    args.Add(string.Format(
                        "Indices.[Name] = '{0}' and Indices.[Value] <= {1}",
                        (filter as Lte).Field.Name,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Lte).Literal);
                    continue;
                }

                if (filter is Gt)
                {
                    args.Add(string.Format(
                        "Indices.[Name] = '{0}' and Indices.[Value] > {1}",
                        (filter as Gt).Field.Name,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Gt).Literal);
                    continue;
                }

                if (filter is Gte)
                {
                    args.Add(string.Format(
                        "Indices.[Name] = '{0}' and Indices.[Value] >= {1}",
                        (filter as Gte).Field.Name,
                        "@p" + Parameters.Count
                    ));
                    Parameters.Add((filter as Gte).Literal);
                    continue;
                }

                if (filter is In)
                {
                    var inf = filter as In;
                    args.Add(string.Format(
                        "Indices.[Name] = '{0}' and Indices.[Value] in ({1})",
                        inf.Field.Name,
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