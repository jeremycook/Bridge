using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBridge.Db.Meta
{
    public class Record
    {
        [Obsolete("Runtime only", true)]
        public Record() { }
        public Record(object model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            FieldIndexes = new List<FieldIndex>();

            Id = (model is IIdentify) ? (model as IIdentify).Id : Guid.NewGuid();
            ClassName = model.GetType().FullName;
            SetModel(model);
        }

        private object _Model;
        private string _Name;
        private Type _ModelType;

        public Guid Id { get; protected set; }

        // TODO? [Index("IX_dbo_Records_ClassName")]
        //[Required, StringLength(250)]
        public string ClassName { get; protected set; }
        //public virtual Class Class { get; private set; }

        //[Index("IX_dbo_Records_Name")]
        //[Required, StringLength(250)]
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

        //[Required, MaxLength(16000)]
        public byte[] Storage { get; protected set; }

        public virtual ICollection<FieldIndex> FieldIndexes { get; set; }


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

            if (model.GetType().FullName != this.ClassName)
                throw new InvalidOperationException(string.Format(
                    "The full type name of `model` ({0}) must match this record's `TypeName` ({1}).",
                    model.GetType().FullName, this.ClassName));

            _Model = model;
            Name = _Model.ToString();
            string json = Serializer.Current.Serialize(_Model);
            Storage = System.Text.Encoding.UTF8.GetBytes(json);

            // Update field indexes.
            FieldIndexes.Clear();
            foreach (var prop in GetModelType().GetProperties())
            {
                FieldIndexes.Add(new FieldIndex(Id, prop.Name, prop.GetValue(model)));
            }
        }

        private Type GetModelType()
        {
            if (_ModelType == null)
            {
                foreach (var assembly in DbBridge.ModelAssemblies)
                {
                    Type type = assembly.GetType(ClassName);
                    if (type != null)
                    {
                        _ModelType = type;
                        break;
                    }
                }

                if (_ModelType == null)
                {
                    throw new TypeLoadException(string.Format("Could not load type '{0}' from any currently loaded assemblies.", ClassName));
                }
            }

            return _ModelType;
        }
    }
}
