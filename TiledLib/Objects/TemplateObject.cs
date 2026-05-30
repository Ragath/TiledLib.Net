namespace TiledLib.Objects;

/// <summary>
/// Represents an object whose shape and properties are defined by an external template file.
/// Per the TMX spec, such objects have a <c>template</c> attribute and no inherent shape element.
/// </summary>
public class TemplateObject : BaseObject
{
    internal TemplateObject(Dictionary<string, string> properties) : base(properties) { }
    public TemplateObject() : base([]) { }
}
