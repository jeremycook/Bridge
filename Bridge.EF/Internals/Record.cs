using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Bridge.EF.Internals
{
    internal class Record
    {
        [Obsolete("Runtime only", true)]
        public Record() { }
        public Record(object model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            InterfaceIndices = new List<InterfaceIndex>();
            FieldIndices = new List<FieldIndex>();

            Id = (model is IIdentify) ? (model as IIdentify).Id : Guid.NewGuid();
            TypeName = model.GetType().FullName;
            SetModel(model);

            foreach (var item in model.GetType().GetInterfaces())
            {
                InterfaceIndices.Add(new InterfaceIndex(item.FullName));
            }

            // TODO: Add field indexes.
        }

        private object _Model;
        private string _Name;

        public Guid Id { get; protected set; }

        [Index("IX_dbo_Records_TypeName")]
        [Required, StringLength(250)]
        public string TypeName { get; protected set; }

        [Index("IX_dbo_Records_Name")]
        [Required, StringLength(250)]
        public string Name
        {
            get { return _Name; }
            protected set
            {
                _Name = value.Length > 250 ?
                    value.Substring(0, 249) + "…" :
                    value;
            }
        }

        [Required, MaxLength(16000)]
        public byte[] Storage { get; protected set; }

        public virtual ICollection<InterfaceIndex> InterfaceIndices { get; protected set; }
        public virtual ICollection<FieldIndex> FieldIndices { get; protected set; }


        /// <summary>
        /// Deserializes <see cref="Storage"/> from JSON.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public object GetModel()
        {
            if (_Model == null && Storage != null)
            {
                string json = System.Text.Encoding.UTF8.GetString(Storage);
                _Model = Serializer.Current.Deserialize(json, GetModelType());
            }
            return _Model;
        }

        /// <summary>
        /// Serializes the value to <see cref="Storage"/> as JSON, and sets <see cref="Name"/>
        /// to the value's string representation.
        /// </summary>
        public void SetModel(object model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.GetType().FullName != this.TypeName)
                throw new InvalidOperationException(string.Format(
                    "The full type name of `model` ({0}) must match this record's `TypeName` ({1}).",
                    model.GetType().FullName, this.TypeName));

            _Model = model;
            Name = _Model.ToString();
            string json = Serializer.Current.Serialize(_Model);
            Storage = System.Text.Encoding.UTF8.GetBytes(json);

            //// Index interfaces.
            //// TODO: Abstract this out into a pluggable system.
            //if (_Model.GetType().IsClass)
            //{
            //    var storedInterfaces = FieldIndexes.Where(o => o.Name == "Interface").ToList();
            //    var modelInterfaceNames = _Model.GetType().GetInterfaces().Select(o => o.FullName).ToList();

            //    var obsoleteInterfaces = storedInterfaces.Where(o => !modelInterfaceNames.Contains(o.Value)).ToList();
            //    foreach (var item in obsoleteInterfaces)
            //    {
            //        FieldIndexes.Remove(item);
            //    }

            //    var newInterfaceNames = modelInterfaceNames.Where(o => !storedInterfaces.Any(i => i.Value == o)).ToList();
            //    foreach (var fullName in newInterfaceNames)
            //    {
            //        FieldIndexes.Add(new Index(Id, "Interface", fullName));
            //    }
            //}
        }

        private Type GetModelType()
        {
            foreach (var assembly in EFBridge.ModelAssemblies)
            {
                Type type = assembly.GetType(TypeName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new TypeLoadException(string.Format("Could not load type '{0}' from any currently loaded assemblies.", TypeName));
        }
    }
}
