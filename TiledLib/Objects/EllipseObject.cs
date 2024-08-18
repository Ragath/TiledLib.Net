﻿using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TiledLib.Objects;

public class EllipseObject : BaseObject, IXmlSerializable
{
    public EllipseObject(Dictionary<string, string> properties) : base(properties) { }
    public EllipseObject() : base([]) { }

    [JsonPropertyName("ellipse")]
    public bool IsEllipse { get; set; }

    public XmlSchema GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {

    }

    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}
