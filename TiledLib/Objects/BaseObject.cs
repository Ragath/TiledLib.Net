namespace TiledLib.Objects;

[JsonConverter(typeof(ObjectConverter))]
public abstract class BaseObject
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string ObjectType { get; set; }


    [JsonPropertyName("visible")]
    public bool Visible { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("x")]
    public double X { get; set; }
    [JsonPropertyName("y")]
    public double Y { get; set; }
    [JsonPropertyName("width")]
    public double Width { get; set; }
    [JsonPropertyName("height")]
    public double Height { get; set; }

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; init; } = [];

    public BaseObject(Dictionary<string, string> properties) { Properties = properties; }
}
