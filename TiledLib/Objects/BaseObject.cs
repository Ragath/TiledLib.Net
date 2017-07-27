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


        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; }
    }
}
