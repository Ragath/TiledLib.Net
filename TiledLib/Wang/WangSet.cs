namespace TiledLib.Wang;

public class WangSet
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Class { get; set; }
    public WangSetType? Type { get; set; }

    /// <summary>
    /// Local ID of the tile representing this Wang set
    /// </summary>
    public int Tile { get; set; } = -1;

    public List<WangColor> Colors { get; set; } = new();

    [JsonPropertyName("wangtiles")]
    public List<WangTile> Tiles { get; set; } = new();

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; init; } = [];
}
