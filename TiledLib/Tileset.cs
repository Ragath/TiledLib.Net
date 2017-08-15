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
        public string name { get; set; }

        public int firstgid { get; set; }

        [JsonProperty("image")]
        public string ImagePath { get; set; }

        public int imageheight { get; set; }
        public int imagewidth { get; set; }

        public int margin { get; set; }
        public int spacing { get; set; }

        public int tileheight { get; set; }
        public int tilewidth { get; set; }

        public string transparentcolor { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        [JsonProperty("tileproperties")]
        public Dictionary<int, Dictionary<string, string>> TileProperties { get; } = new Dictionary<int, Dictionary<string, string>>();

        public string this[int id, string property]
        {
            get
            {
                if (id != 0 && TileProperties.TryGetValue(id - firstgid, out var tile) && tile.TryGetValue(property, out var value))
                    return value;
                else
                    return null;
            }
        }

        public Tile this[int id]
        {
            get
            {
                if (id == 0)
                    return default(Tile);

                var orientation = (TileOrientation)id & TileOrientation.MaskFlip;

                var columns = Columns;
                var rows = Rows;
                var index = ((int)TileOrientation.MaskID & id) - firstgid;
                if (index < 0 || index >= rows * columns)
                    throw new ArgumentOutOfRangeException();

                var row = index / columns;

                return new Tile
                {
                    Top = row * (tileheight + spacing) + margin,
                    Left = (index - row * columns) * (tilewidth + spacing) + margin,
                    Width = tilewidth,
                    Height = tileheight,
                    Orientation = orientation
                };
            }
        }

        public int Columns => (imagewidth + spacing - margin * 2) / (tilewidth + spacing);
        public int Rows => (imageheight + spacing - margin * 2) / (tileheight + spacing);
        public int TileCount => Columns * Rows;

        [JsonIgnore] //TODO: Add json support
        public Dictionary<int, Frame[]> TileAnimations { get; } = new Dictionary<int, Frame[]>();

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