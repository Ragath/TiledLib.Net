using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TiledLib.Layer
{
    [XmlRoot("layer")]
    public class TileLayer : BaseLayer, IXmlSerializable
    {
        [JsonProperty("encoding")]
        public string Encoding { get; set; }

        [JsonProperty("compression")]
        public string Compression { get; set; }

        [JsonRequired]
        public int[] data { get; set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            LayerType = LayerType.tilelayer;
            reader.ReadLayerAttributes(this);

            reader.Read();
            if (!reader.IsStartElement("data"))
                throw new XmlException();

            data = TmxParsing.ReadData(reader, Width * Height, out var encoding, out var compression);
            Encoding = encoding;
            Compression = compression;
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}