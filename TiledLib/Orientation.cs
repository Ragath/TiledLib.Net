namespace TiledLib;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Orientation
{
    unknown = default,
    orthogonal,
    isometric,
    hexagonal
};
