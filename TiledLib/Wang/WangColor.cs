namespace TiledLib.Wang;

public class WangColor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Class { get; set; }
    /// <summary>
    /// hex-formatted (#RRGGBB or #AARRGGBB)
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Local ID of the tile representing this Wang color
    /// </summary>
    public int Tile { get; set; } = -1;

    /// <summary>
    /// Relative probability used when randomizing (default: 1)
    /// </summary>
    public double Probability { get; set; } = 1;

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; init; } = [];
}
