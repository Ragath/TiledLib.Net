using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TiledLib;

[XmlRoot("tileset")]
public class Tileset : ITileset, IXmlSerializable
{
    public string Name { get; set; }

    public int FirstGid { get; set; }

    [JsonPropertyName("image")]
    public string ImagePath { get; set; }

    [JsonPropertyName("imageheight")]
    public int ImageHeight { get; set; }
    [JsonPropertyName("imagewidth")]
    public int ImageWidth { get; set; }

    public int Margin { get; set; }
    public int Spacing { get; set; }

    [JsonPropertyName("tileheight")]
    public int TileHeight { get; set; }
    [JsonPropertyName("tilewidth")]
    public int TileWidth { get; set; }

    public Grid Grid { get; set; }

    [JsonPropertyName("transparentcolor")]
    public string TransparentColor { get; set; }

    [JsonPropertyName("tileoffset")]
    public TileOffset TileOffset { get; set; }

    [JsonPropertyName("properties")]
    [JsonConverter(typeof(PropertiesConverter))]
    public Dictionary<string, string> Properties { get; init; } = new Dictionary<string, string>();
    [JsonPropertyName("tileproperties")]
    public Dictionary<int, Dictionary<string, string>> TileProperties { get; init; } = new Dictionary<int, Dictionary<string, string>>();
    public Dictionary<int, string> TileSetImages { get; init; } = new Dictionary<int, string>();

    public bool IsImageCollection => TileSetImages.Count > 0;


    [JsonIgnore] //TODO: Add json support
    public Dictionary<int, Frame[]> TileAnimations { get; init; } = new Dictionary<int, Frame[]>();

    public Tile this[uint gid]
    {
        get
        {
            if (gid == 0)
                return default;

            var orientation = Utils.GetOrientation(gid);

            var columns = Columns;
            var rows = Rows;
            var index = Utils.GetId(gid) - (int)FirstGid;
            if (index < 0 || index >= rows * columns)
                throw new ArgumentOutOfRangeException();

            var row = index / columns;

            return new Tile
            {
                Top = row * (TileHeight + Spacing) + Margin,
                Left = (index - row * columns) * (TileWidth + Spacing) + Margin,
                Width = TileWidth,
                Height = TileHeight,
                Orientation = orientation
            };
        }
    }

    public string this[uint gid, string property]
    {
        get
        {
            var id = Utils.GetId(gid);
            return id != 0
                   && TileProperties.TryGetValue(id - FirstGid, out var tile)
                   && tile.TryGetValue(property, out var value) ? value : default;
        }
    }

    public int Columns => (ImageWidth + Spacing - Margin * 2) / (TileWidth + Spacing);
    public int Rows => (ImageHeight + Spacing - Margin * 2) / (TileHeight + Spacing);
    public int TileCount => Columns * Rows;

    public static Tileset FromStream(Stream stream)
    {
        if (Utils.ContainsJson(stream))
            return JsonSerializer.Deserialize<Tileset>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        else
            return (Tileset)new XmlSerializer(typeof(Tileset)).Deserialize(stream);

    }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadTileset(this);
    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

}