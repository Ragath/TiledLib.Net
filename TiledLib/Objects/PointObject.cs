namespace TiledLib.Objects;

public class PointObject : BaseObject
{
    internal PointObject(Dictionary<string, string> properties) : base(properties) { }
    public PointObject() : base(new Dictionary<string, string>()) { }
}
