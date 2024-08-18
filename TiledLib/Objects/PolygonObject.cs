namespace TiledLib.Objects;

public class PolygonObject : BaseObject
{
    internal PolygonObject(Dictionary<string, string> properties) : base(properties) { }
    public PolygonObject() : base([]) { }

    public Position[] Polygon { get; set; }
}
