using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib.Objects
{
    [JsonConverter(typeof(ObjectConverter))]
    public abstract class BaseObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string ObjectType { get; set; }


        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }
        [JsonProperty("y")]
        public double Y { get; set; }
        [JsonProperty("width")]
        public double Width { get; set; }
        [JsonProperty("height")]
        public double Height { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public BaseObject(Dictionary<string, string> properties) { Properties = properties; }
    }
}
