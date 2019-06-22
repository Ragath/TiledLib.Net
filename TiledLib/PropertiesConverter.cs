using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TiledLib
{
    public class PropertiesConverter : JsonConverter
    {
        protected class Property
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(Dictionary<string, string>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
                return serializer.Deserialize<Property[]>(reader).ToDictionary(key => key.Name, value => value.Value);
            else
                return serializer.Deserialize<Dictionary<string, string>>(reader);
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }

}
