using System.ComponentModel.DataAnnotations;

namespace TiledLib.Layer;

[JsonConverter(typeof(LayerConverter))]
public abstract class BaseLayer
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Required, JsonPropertyName("type")]
    public LayerType LayerType { get; set; }


    [JsonPropertyName("opacity")]
    public double Opacity { get; set; }

    [JsonPropertyName("visible")]
    public bool Visible { get; set; }


    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("x")]
    [Obsolete("Use OffsetX instead.")]
    public int X { get; set; }
    [JsonPropertyName("y")]
    [Obsolete("Use OffsetY instead.")]
    public int Y { get; set; }

    [JsonPropertyName("offsetx")]
    public double OffsetX { get; set; }
    [JsonPropertyName("offsety")]
    public double OffsetY { get; set; }

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; init; } = [];
}
