using System;

namespace DBridge
{
    public class Literal
    {
        public Literal(Guid guid)
        {
            this.ValueType = LiteralType.Guid;
            this.Value = guid;
        }

        public Literal(DateTimeOffset moment)
        {
            this.ValueType = LiteralType.Moment;
            this.Value = moment;
        }

        public Literal(string text)
        {
            this.ValueType = LiteralType.Text;
            this.Value = text;
        }

        public Literal(int integer)
        {
            this.ValueType = LiteralType.Number;
            this.Value = integer;
        }

        public Literal(decimal @decimal)
        {
            this.ValueType = LiteralType.Number;
            this.Value = @decimal;
        }

        public Literal(float @float)
        {
            this.ValueType = LiteralType.Float;
            this.Value = @float;
        }

        public LiteralType ValueType { get; protected set; }
        public object Value { get; protected set; }
    }

    public enum LiteralType
    {
        Guid = 1,
        Text = 2,
        Moment = 3,
        Number = 4,
        Float = 5,
    }
}