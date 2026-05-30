using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TiledLib.Objects;

namespace TiledLib;

/// <summary>
/// Represents a Tiled object template (.tx file).
/// A template holds an optional tileset reference and a single object definition.
/// </summary>
[XmlRoot("template")]
public class Template : IXmlSerializable
{
    /// <summary>
    /// The tileset used by the template object, if any (only present for tile objects).
    /// </summary>
    [JsonPropertyName("tileset")]
    public ExternalTileset? Tileset { get; set; }

    /// <summary>
    /// The object defined by this template.
    /// </summary>
    [JsonPropertyName("object")]
    public BaseObject? Object { get; set; }

    public static Template FromStream(Stream stream)
    {
        if (Utils.ContainsJson(stream))
            return JsonSerializer.Deserialize<Template>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new NullReferenceException();
        else
            return (Template)new XmlSerializer(typeof(Template)).Deserialize(stream)!;
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement("template");
        while (reader.IsStartElement())
            switch (reader.Name)
            {
                case "tileset":
                    Tileset = new ExternalTileset
                    {
                        Source = reader["source"] ?? throw new XmlException("tileset missing source"),
                        FirstGid = int.Parse(reader["firstgid"] ?? "0"),
                    };
                    reader.Skip();
                    break;
                case "object":
                    Object = reader.ReadObject(isTemplate: true);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        if (reader.Name == "template")
            reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Tileset != null)
        {
            writer.WriteStartElement("tileset");
            writer.WriteAttribute("firstgid", Tileset.FirstGid);
            writer.WriteAttribute("source", Tileset.Source ?? throw new NullReferenceException("tileset missing source"));
            writer.WriteEndElement();
        }

        if (Object != null)
            writer.WriteObject(Object, isTemplate: true);
    }
}
