namespace TiledLib.Objects;

public class PolyLineObject : BaseObject
{
    internal PolyLineObject(Dictionary<string, string> properties) : base(properties) { }
    public PolyLineObject() : base([]) { }

    public Position[] Polyline { get; set; }
}
