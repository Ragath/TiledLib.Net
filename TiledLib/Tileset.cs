using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TiledLib
{
    [XmlRoot("tileset")]
    public class Tileset : ITileset, IXmlSerializable
    {
        public string Name { get; set; }

        public int FirstGid { get; set; }

        [JsonProperty("image")]
        public string ImagePath { get; set; }

        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }

        public int Margin { get; set; }
        public int Spacing { get; set; }

        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        public Grid Grid { get; set; }

        public string TransparentColor { get; set; }

        [JsonProperty("tileoffset")]
        public TileOffset TileOffset { get; set; }

        [JsonProperty("properties")]
        [JsonConverter(typeof(PropertiesConverter))]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        [JsonProperty("tileproperties")]
        public Dictionary<int, Dictionary<string, string>> TileProperties { get; } = new Dictionary<int, Dictionary<string, string>>();



        [JsonIgnore] //TODO: Add json support
        public Dictionary<int, Frame[]> TileAnimations { get; } = new Dictionary<int, Frame[]>();

        public Tile this[int gid]
        {
            get
            {
                if (gid == 0)
                    return default;

                var orientation = Utils.GetOrientation(gid);

                var columns = Columns;
                var rows = Rows;
                var index = Utils.GetId(gid) - FirstGid;
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

        public string this[int gid, string property]
        {
            get
            {
                gid = Utils.GetId(gid);
                return gid != 0
                       && TileProperties.TryGetValue(gid - FirstGid, out var tile)
                       && tile.TryGetValue(property, out var value) ? value : default;
            }
        }

        public int Columns => (ImageWidth + Spacing - Margin * 2) / (TileWidth + Spacing);
        public int Rows => (ImageHeight + Spacing - Margin * 2) / (TileHeight + Spacing);
        public int TileCount => Columns * Rows;

        public static Tileset FromStream(System.IO.Stream stream)
        {
            using (var reader = new System.IO.StreamReader(stream))
            {
                if (Utils.ContainsJson(reader))
                    return (Tileset)Utils.JsonSerializer.Deserialize(reader, typeof(Tileset));
                else
                    return (Tileset)new XmlSerializer(typeof(Tileset)).Deserialize(reader);
            }
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
}