namespace TiledLib;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LayerType
{
    unknown = default,
    tilelayer,
    objectgroup,
    imagelayer
};