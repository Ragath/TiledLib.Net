using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TiledLib
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RenderOrder : byte
    {
        Unknown = 0,
        [EnumMember(Value = "left-up")]
        leftup,
        [EnumMember(Value = "left-down")]
        leftdown,
        [EnumMember(Value = "right-up")]
        rightup,
        [EnumMember(Value = "right-down")]
        rightdown
    }
}