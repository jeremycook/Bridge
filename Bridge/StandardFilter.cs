using System;

namespace Bridge
{
    public abstract class StandardFilter
    {
    }

    public class And : StandardFilter
    {
        public And(StandardFilter leftFilter, StandardFilter rightFilter)
        {
            this.LeftFilter = leftFilter;
            this.RightFilter = rightFilter;
        }

        public StandardFilter LeftFilter { get; protected set; }
        public StandardFilter RightFilter { get; protected set; }
    }

    public class Or : StandardFilter
    {
        public Or(StandardFilter leftFilter, StandardFilter rightFilter)
        {
            this.LeftFilter = leftFilter;
            this.RightFilter = rightFilter;
        }

        public StandardFilter LeftFilter { get; protected set; }
        public StandardFilter RightFilter { get; protected set; }
    }

    public class Not : StandardFilter
    {
        public Not(StandardFilter filter)
        {
            this.Filter = filter;
        }

        public StandardFilter Filter { get; protected set; }
    }

    public class Eq : StandardFilter
    {
        public Eq(Field field, Literal literal)
        {
            this.Field = field;
            this.Literal = literal;
        }

        public Field Field { get; protected set; }
        public Literal Literal { get; protected set; }
    }

    public class Lt : StandardFilter
    {
        public Lt(Field field, Literal literal)
        {
            this.Field = field;
            this.Literal = literal;
        }

        public Field Field { get; protected set; }
        public Literal Literal { get; protected set; }
    }

    public class Lte : StandardFilter
    {
        public Lte(Field field, Literal literal)
        {
            this.Field = field;
            this.Literal = literal;
        }

        public Field Field { get; protected set; }
        public Literal Literal { get; protected set; }
    }

    public class Gt : StandardFilter
    {
        public Gt(Field field, Literal literal)
        {
            this.Field = field;
            this.Literal = literal;
        }

        public Field Field { get; protected set; }
        public Literal Literal { get; protected set; }
    }

    public class Gte : StandardFilter
    {
        public Gte(Field field, Literal literal)
        {
            this.Field = field;
            this.Literal = literal;
        }

        public Field Field { get; protected set; }
        public Literal Literal { get; protected set; }
    }

    public class In : StandardFilter
    {
        public In(Field field, params Literal[] literals)
        {
            this.Field = field;
            this.Literals = literals;
        }

        public Field Field { get; protected set; }
        public Literal[] Literals { get; protected set; }
    }
}