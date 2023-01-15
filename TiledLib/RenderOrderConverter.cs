namespace TiledLib;

public class RenderOrderConverter : JsonConverter<RenderOrder>
{
    public override RenderOrder Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.ToLowerInvariant() switch
        {
            "left-up" => RenderOrder.leftup,
            "left-down" => RenderOrder.leftdown,
            "right-up" => RenderOrder.rightup,
            "right-down" => RenderOrder.rightdown,
            _ => RenderOrder.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, RenderOrder value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}