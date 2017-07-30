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
            byte[] bytes = null;
            BaseLayer result;
            switch (jo["type"].Value<string>())
            {
                case "tilelayer":
                    result = new TileLayer();
                    if (jo["encoding"].Value<string>() == "base64")
                    {
                        bytes = Convert.FromBase64String(jo["data"].Value<string>());
                        jo["data"].Replace(JToken.FromObject(new int[0]));
                    }
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

            if (result is TileLayer tl && bytes != null)
                switch (tl.Compression)
                {
                    case null:
                        tl.data = new int[bytes.Length / sizeof(int)];
                        Buffer.BlockCopy(bytes, 0, tl.data, 0, bytes.Length);
                        break;
                    default:
                        throw new NotImplementedException($"Compression: {tl.Compression}");
                }


            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
