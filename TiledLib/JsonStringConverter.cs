namespace TiledLib;

public class JsonStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDecimal().ToString(),
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.True or JsonTokenType.False => reader.GetBoolean().ToString(),
            _ => reader.GetString(),
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => throw new NotImplementedException();
}
