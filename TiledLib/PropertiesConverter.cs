

namespace TiledLib;

public class PropertiesConverter : JsonConverter<Dictionary<string, string>>
{
    protected class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
            return JsonSerializer.Deserialize<Property[]>(ref reader).ToDictionary(key => key.Name, value => value.Value);
        else
            return JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
