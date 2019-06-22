using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TiledLib.Objects
{
    class ObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(BaseObject);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            BaseObject result;
            if (jo.Property("gid") != null)
                result = new TileObject();
            else if (jo.Property("polygon") != null)
                result = new PolygonObject();
            else if (jo.Property("polyline") != null)
                result = new PolyLineObject();
            else if (jo.Value<bool>("ellipse"))
                result = new EllipseObject();
            else
                result = new RectangleObject();

            serializer.Populate(jo.CreateReader(), result);

            return result;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) 
            => throw new NotImplementedException();
    }
}
