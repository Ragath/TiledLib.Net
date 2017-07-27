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
            LayerType = LayerType.objectgroup;

            reader.ReadLayerAttributes(this);

            Objects = reader.ReadObjectLayerElements().ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }

}
