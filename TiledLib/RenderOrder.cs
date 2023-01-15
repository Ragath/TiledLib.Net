using System.Runtime.Serialization;

namespace TiledLib;

[JsonConverter(typeof(RenderOrderConverter))]
public enum RenderOrder : byte
{
    Unknown = default,
    [EnumMember(Value = "left-up")]
    leftup,
    [EnumMember(Value = "left-down")]
    leftdown,
    [EnumMember(Value = "right-up")]
    rightup,
    [EnumMember(Value = "right-down")]
    rightdown
}
