using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayerType
    {
        unknown = default,
        tilelayer,
        objectgroup,
        imagelayer
    };
}