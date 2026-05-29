namespace TiledLib.Wang;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WangSetType
{
    corner,
    edge,
    mixed
}