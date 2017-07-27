using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TiledLib.Layer;

namespace TiledLib
{
    [XmlRoot("map")]
    public class Map : IXmlSerializable
    {
        [JsonRequired]
        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty("tiledversion")]
        public string TiledVersion { get; set; }

        [JsonRequired]
        [JsonProperty("orientation")]
        public Orientation Orientation { get; set; }

        [JsonProperty("renderorder")]
        public RenderOrder RenderOrder { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("tilewidth")]
        public int CellWidth { get; set; }
        [JsonProperty("tileheight")]
        public int CellHeight { get; set; }

        [JsonProperty("layers")]
        public BaseLayer[] Layers { get; set; }

        [JsonProperty("tilesets")]
        public ITileset[] Tilesets { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Parses map from JSON stream
        /// </summary>
        /// <param name="stream">JSON stream</param>
        /// <returns>Tiled Map</returns>
        public static Map FromStream(Stream stream, Func<ExternalTileset, Stream> tsLoader = null)
        {
            using (var reader = new StreamReader(stream))
            {
                var map = reader.ContainsJson() ? reader.ReadJsonMap() : reader.ReadTmxMap();

                if (tsLoader != null)
                    foreach (var item in map.Tilesets)
                        if (item is ExternalTileset e)
                            e._Tileset = new Lazy<Tileset>(() =>
                            {
                                using (var s = tsLoader(e))
                                {
                                    return Tileset.FromStream(s);
                                }
                            });
                return map;
            }
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ReadMapAttributes(this);
            reader.ReadMapElements(this);
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}