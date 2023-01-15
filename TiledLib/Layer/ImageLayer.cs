using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TiledLib.Layer;

[XmlRoot("imagelayer")]
public class ImageLayer : BaseLayer, IXmlSerializable
{
    [JsonPropertyName("image")]
    public string Image { get; set; }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        if (!reader.IsStartElement("imagelayer"))
            throw new XmlException();

        LayerType = LayerType.imagelayer;

        Id = reader["id"].ParseInt32().GetValueOrDefault();
        Name = reader["name"] ?? throw new KeyNotFoundException("name");
        Visible = reader["visible"].ParseBool() ?? true;
        Opacity = reader["opacity"].ParseDouble() ?? 1.0;

        OffsetX = reader["offsetx"].ParseDouble().GetValueOrDefault();
        OffsetY = reader["offsety"].ParseDouble().GetValueOrDefault();

        if (reader.ReadToDescendant("image"))
        {
            Image = reader["source"] ?? throw new KeyNotFoundException("source");
            Width = reader["width"].ParseInt32() ?? -1;
            Height = reader["height"].ParseInt32() ?? -1;
            reader.Read();
        }

        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer) 
        => throw new NotImplementedException();
}
