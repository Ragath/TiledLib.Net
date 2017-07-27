using Newtonsoft.Json;

namespace TiledLib.Objects
{
    class TileObject : BaseObject
    {
        [JsonProperty("gid")]
        public int gId { get; set; }
    }
}
