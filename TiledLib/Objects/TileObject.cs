namespace TiledLib.Objects;

public class TileObject : BaseObject
{
    internal TileObject(Dictionary<string, string> properties) : base(properties) { }
    public TileObject() : base([]) { }

    [JsonPropertyName("gid")]
    public int Gid { get; set; }
}
