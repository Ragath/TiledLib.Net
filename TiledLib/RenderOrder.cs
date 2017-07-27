using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RenderOrder : byte
    {
        Unknown = 0,
        [EnumMember(Value = "right-down")]
        rightdown
    }
}