using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayerType
    {
        unknown = 0,
        tilelayer,
        objectgroup,
        imagelayer
    };
}