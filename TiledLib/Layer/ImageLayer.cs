using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TiledLib.Layer
{
    [XmlRoot("imagelayer")]
    public class ImageLayer : BaseLayer, IXmlSerializable
    {
        [JsonProperty("image")]
        public string Image { get; set; }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            //<imagelayer name="Image Layer 1" visible="0" opacity="0.63">
            //   <image source="sewer_tileset.png" width="192" height="217"/>
            //</imagelayer>
            LayerType = LayerType.imagelayer;
            Name = reader["name"] ?? throw new KeyNotFoundException("name");
            Visible = reader["visible"].ParseBool() ?? true;
            Opacity = reader["opacity"].ParseDouble() ?? 1.0;
            if (reader.ReadToDescendant("image"))
            {
                Image = reader["source"] ?? throw new KeyNotFoundException("source");
                Width = reader["width"].ParseInt32() ?? throw new KeyNotFoundException("width");
                Height = reader["height"].ParseInt32() ?? throw new KeyNotFoundException("height");
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
