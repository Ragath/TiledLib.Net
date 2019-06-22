using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib.Layer
{
    [JsonConverter(typeof(LayerConverter))]
    public abstract class BaseLayer
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Id { get; set; }

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
        [Obsolete("Use OffsetX instead.")]
        public int X { get; set; }
        [JsonProperty("y")]
        [Obsolete("Use OffsetY instead.")]
        public int Y { get; set; }

        [JsonProperty("offsetx")]
        public double OffsetX { get; set; }
        [JsonProperty("offsety")]
        public double OffsetY { get; set; }

        [JsonProperty("properties")]
        [JsonConverter(typeof(PropertiesConverter))]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
    }
}
