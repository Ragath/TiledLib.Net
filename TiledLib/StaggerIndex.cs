using System.Runtime.Serialization;

namespace TiledLib;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StaggerIndex : byte
{
    None = default,
    [EnumMember(Value = "odd")]
    odd,
    [EnumMember(Value = "even")]
    even,
}