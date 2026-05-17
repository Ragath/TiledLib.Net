namespace TiledLib;


public class TilesetConverter : JsonConverter<ITileset>
{

    public override ITileset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jo = JsonElement.ParseValue(ref reader);
        return jo.TryGetProperty("source", out var _) switch
        {
            true => jo.Deserialize<ExternalTileset>(options),
            false => jo.Deserialize<Tileset>(options)
        };
    }

    public override void Write(Utf8JsonWriter writer, ITileset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}