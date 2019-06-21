using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Orientation
    {
        unknown = default,
        orthogonal,
        isometric,
        hexagonal
    };
}
