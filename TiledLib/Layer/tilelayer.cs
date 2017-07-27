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
            switch (reader["encoding"])
            {
                case "csv":
                    data = reader.ReadElementContentAsString().Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                    if (data.Length != Width * Height)
                        throw new XmlException();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}