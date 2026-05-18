using System.Xml;
using System.Xml.Serialization;

namespace TiledLib.Layer;

[XmlRoot("layer")]
public class ChunkLayer : TileLayer, IXmlSerializable
{
    public Chunk[]? Chunks { get; set; }

    public override void ReadXml(XmlReader reader)
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
                    {
                        foundData = true;

                        Encoding = reader["encoding"];
                        Compression = reader["compression"];
                        var chunks = new List<Chunk>();
                        reader.Read();
                        using var subReader = reader.ReadSubtree();
                        while (subReader.Read())
                        {
                            subReader.MoveToElement();
                            switch (subReader.Name)
                            {
                                case "chunk":
                                    var chunkData = subReader.ReadChunk(Encoding, Compression);
                                    chunks.Add(chunkData);
                                    break;
                                default:
                                    throw new XmlException(subReader.Name);
                            }
                        }
                        Chunks = [.. chunks];
                        reader.ReadEndElement();
                        reader.ReadEndElement();
                    }
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
}