using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TiledLib.Layer;

namespace TiledLib;

[XmlRoot("map")]
public class Map : IXmlSerializable
{
    [Required]
    [JsonPropertyName("version")]
    [JsonConverter(typeof(VersionConverter))]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("tiledversion")]
    public string TiledVersion { get; set; }

    [Required]
    [JsonPropertyName("orientation")]
    public Orientation Orientation { get; set; }

    [JsonPropertyName("renderorder")]
    public RenderOrder RenderOrder { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("tilewidth")]
    public int CellWidth { get; set; }
    [JsonPropertyName("tileheight")]
    public int CellHeight { get; set; }

    [JsonPropertyName("hexsidelength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? HexSideLength { get; set; }


    [JsonPropertyName("infinite")]
    public bool Infinite { get; set; }

    [JsonPropertyName("nextlayerid")]
    public int NextLayerId { get; set; }

    [JsonPropertyName("nextobjectid")]
    public int NextObjectId { get; set; }

    [JsonPropertyName("layers")]
    public BaseLayer[] Layers { get; set; }

    [JsonPropertyName("tilesets")]
    public ITileset[] Tilesets { get; set; }

    [JsonPropertyName("staggeraxis")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public StaggerAxis StaggerAxis { get; set; }

    [JsonPropertyName("staggerindex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public StaggerIndex StaggerIndex { get; set; }

    [JsonPropertyName("backgroundcolor")]
    public string BackgroundColor { get; set; }

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Parses map from JSON stream
    /// </summary>
    /// <param name="stream">JSON stream</param>
    /// <returns>Tiled Map</returns>
    public static Map FromStream(Stream stream, Func<ExternalTileset, Stream> tsLoader = null)
    {
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true, 1024, true);
        var map = reader.ContainsJson() ? reader.BaseStream.ReadJsonMap() : reader.ReadTmxMap();

        if (tsLoader != null)
            foreach (var item in map.Tilesets)
                if (item is ExternalTileset ets)
                    ets.LoadTileset(e =>
                    {
                        using var s = tsLoader(e);
                        return Tileset.FromStream(s);
                    });
        return map;
    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadMapAttributes(this);
        reader.ReadMapElements(this);
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteMapAttributes(this);
        writer.WriteMapElements(this);
        //HACK: throw new NotImplementedException();
    }
}