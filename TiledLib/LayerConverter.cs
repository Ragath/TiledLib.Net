using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TiledLib.Layer;

namespace TiledLib
{
    public class LayerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) 
            => objectType == typeof(BaseLayer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            BaseLayer result;
            switch (jo["type"].Value<string>())
            {
                case "tilelayer":
                    result = new TileLayer();
                    break;
                case "objectgroup":
                    result = new ObjectLayer();
                    break;
                case "imagelayer":
                    result = new ImageLayer();
                    break;
                default:
                    return null;
            }
            serializer.Populate(jo.CreateReader(), result);

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
