using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib.Objects
{
    public class TileObject : BaseObject
    {
        internal TileObject(Dictionary<string, string> properties) : base(properties) { }
        public TileObject() : base(new Dictionary<string, string>()) { }

        [JsonProperty("gid")]
        public int gId { get; set; }
    }
}
