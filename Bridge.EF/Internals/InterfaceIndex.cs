using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bridge.EF.Internals
{
    /// <summary>
    /// A value that identifies an inteface that a <see cref="Domain.Record"/> model implements.
    /// </summary>
    internal class InterfaceIndex
    {
        [Obsolete("Runtime only", true)]
        public InterfaceIndex() { }
        public InterfaceIndex(Guid recordId, string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            RecordId = recordId;
            Name = name;
        }

        [Key, Column(Order = 1)]
        [Required]
        public Guid RecordId { get; protected set; }
        public Record Record { get; protected set; }

        [Key, Column(Order = 2)]
        [Required, StringLength(100)]
        public string Name { get; protected set; }
    }
}