using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBridge
{
    public class Serializer
    {
        private static Serializer _Current;

        /// <summary>
        /// Customize the current static serializer.
        /// </summary>
        public static Serializer Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Serializer();
                }
                return _Current;
            }
            set { _Current = value; }
        }

        private readonly JsonSerializerSettings settings;

        public Serializer() : this(settings: new JsonSerializerSettings()) { }
        public Serializer(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }


        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, settings);
        }

        public async Task<object> DeserializeAsync(string json, Type type)
        {
            return await Task.Factory.StartNew(() => Deserialize(json, type));
        }


        public TModel Deserialize<TModel>(string json)
        {
            return JsonConvert.DeserializeObject<TModel>(json, settings);
        }

        public async Task<TModel> DeserializeAsync<TModel>(string json)
        {
            return await Task.Factory.StartNew(() => Deserialize<TModel>(json));
        }


        public string Serialize<TModel>(TModel model)
        {
            return JsonConvert.SerializeObject(model, settings);
        }

        public async Task<string> SerializeAsync<TModel>(TModel model)
        {
            return await Task.Factory.StartNew(() => Serialize(model));
        }
    }
}
