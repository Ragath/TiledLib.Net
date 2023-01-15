namespace TiledLib;


public class TilesetConverter : JsonConverter<ITileset>
{

    public override ITileset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jo = JsonElement.ParseValue(ref reader);

        ITileset result;
        if (jo.TryGetProperty("source", out var _))
            result = jo.Deserialize<ExternalTileset>(options);
        else
            result = jo.Deserialize<Tileset>(options);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, ITileset value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}