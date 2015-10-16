using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBridge.EF.Internals
{
    /// <summary>
    /// A value that identifies an inteface that a <see cref="Domain.Class"/> implements.
    /// </summary>
    internal class Interface
    {
        [Obsolete("Runtime only", true)]
        public Interface() { }
        public Interface(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        [Key, Column(Order = 1)]
        public string ClassName { get; protected set; }
        public virtual Class Class { get; protected set; }

        [Key, Column(Order = 2)]
        [Required, StringLength(100)]
        public string Name { get; protected set; }
    }
}