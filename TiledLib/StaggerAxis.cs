using System.Runtime.Serialization;

namespace TiledLib;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StaggerAxis : byte
{
    None = default,
    [EnumMember(Value = "x")]
    x,
    [EnumMember(Value = "y")]
    y,
}