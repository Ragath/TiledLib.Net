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
            if (!reader.IsStartElement("layer"))
                throw new XmlException();

            LayerType = LayerType.tilelayer;
            reader.ReadLayerAttributes(this);

            var foundData = false;
            reader.ReadStartElement("layer");
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "properties":
                        reader.ReadProperties(Properties);
                        break;
                    case "data":
                        foundData = true;
                        data = reader.ReadData(Width * Height, out var encoding, out var compression);
                        Encoding = encoding;
                        Compression = compression;
                        break;
                    default:
                        throw new XmlException(reader.Name);
                }

            if (reader.Name == "layer")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);

            if (!foundData)
                throw new XmlException("data missing in layer");

        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}