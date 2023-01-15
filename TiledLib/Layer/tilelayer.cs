using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TiledLib.Layer;

[XmlRoot("layer")]
public class TileLayer : BaseLayer, IXmlSerializable
{
    [JsonPropertyName("encoding")]
    public string Encoding { get; set; }

    [JsonPropertyName("compression")]
    public string Compression { get; set; }

    [Required]
    [JsonPropertyName("data")]
    public int[] Data { get; set; }

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
                    Data = reader.ReadData(Width * Height, out var encoding, out var compression);
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