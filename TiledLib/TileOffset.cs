using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TiledLib
{
    public class TileOffset
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }
    }
}
