using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TiledLib.Objects;

namespace TiledLib.Layer
{
    [XmlRoot("objectgroup")]
    public class ObjectLayer : BaseLayer, IXmlSerializable
    {
        [JsonRequired, JsonProperty("objects")]
        public BaseObject[] Objects { get; set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsStartElement("objectgroup"))
                throw new XmlException(reader.Name);

            LayerType = LayerType.objectgroup;

            reader.ReadLayerAttributes(this);
            Objects = reader.ReadObjectLayerElements().ToArray();
            if (reader.Name == "objectgroup")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
