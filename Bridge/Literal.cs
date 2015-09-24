using System;

namespace Bridge
{
    public class Literal
    {
        public Literal(DateTimeOffset moment)
        {
            this.ValueType = typeof(DateTimeOffset);
            this.Value = moment;
        }

        public Literal(string text)
        {
            this.ValueType = typeof(string);
            this.Value = text;
        }

        public Literal(int integer)
        {
            this.ValueType = typeof(int);
            this.Value = integer;
        }

        public Literal(decimal @decimal)
        {
            this.ValueType = typeof(decimal);
            this.Value = @decimal;
        }

        public Literal(float @float)
        {
            this.ValueType = typeof(float);
            this.Value = @float;
        }

        public Type ValueType { get; protected set; }
        public object Value { get; protected set; }
    }

}