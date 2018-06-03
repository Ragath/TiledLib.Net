using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Orientation
    {
        unknown = 0,
        orthogonal,
        isometric,
        hexagonal
    };
}
