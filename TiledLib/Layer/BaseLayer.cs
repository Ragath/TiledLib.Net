using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib.Layer
{
    [JsonConverter(typeof(LayerConverter))]
    public abstract class BaseLayer
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonRequired, JsonProperty("type")]
        public LayerType LayerType { get; set; }


        [JsonProperty("opacity")]
        public double Opacity { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }


        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }


        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();        
    }
}
