using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataBridge.EF.Internals
{
    internal class Class
    {
        public Class() { }
        public Class(Type @class)
        {
            this.Interfaces = new List<Interface>();

            this.Name = @class.FullName;
            this.Update(@class);
        }

        [Key]
        [Required, StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Interface> Interfaces { get; set; }

        /// <summary>
        /// Indexes interfaces.
        /// </summary>
        /// <param name="@class"></param>
        public void Update(Type @class)
        {
            if (!@class.IsClass)
            {
                throw new ArgumentException(nameof(@class), "The argument must be a class.");
            }

            if (@class.FullName != Name)
            {
                throw new ArgumentException(nameof(@class), "The argument's `FullName` must match class's `Name`.");
            }

            var storedInterfaces = Interfaces.ToList();
            var modelInterfaceNames = @class.GetInterfaces().Select(o => o.FullName).ToList();

            var obsoleteInterfaces = storedInterfaces.Where(o => !modelInterfaceNames.Contains(o.Name)).ToList();
            foreach (var item in obsoleteInterfaces)
            {
                Interfaces.Remove(item);
            }

            var newInterfaceNames = modelInterfaceNames.Where(o => !storedInterfaces.Any(i => i.Name == o)).ToList();
            foreach (var fullName in newInterfaceNames)
            {
                Interfaces.Add(new Interface(fullName));
            }
        }
    }
}