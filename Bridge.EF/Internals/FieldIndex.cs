using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bridge.EF.Internals
{
    /// <summary>
    /// A value that identifies and is used to locate a particular element within a <see cref="Domain.Record"/>.
    /// </summary>
    internal class FieldIndex
    {
        [Obsolete("Runtime only", true)]
        public FieldIndex() { }
        public FieldIndex(Guid recordId, string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            RecordId = recordId;
            Name = name;
            Text = value;
        }

        [Key, Column(Order = 1)]
        [Required]
        public Guid RecordId { get; protected set; }
        public Record Record { get; protected set; }

        [Key, Column(Order = 2)]
        [Required, StringLength(100)]
        public string Name { get; protected set; }

        [Key, Column(Order = 3)]
        public string Text { get; protected set; }

        [Key, Column(Order = 4)]
        public DateTimeOffset? Moment { get; protected set; }

        [Key, Column(Order = 5)]
        public decimal? Number { get; protected set; }

        [Key, Column(Order = 6)]
        public float? Float { get; protected set; }
    }
}