namespace TiledLib;

/// <summary>
/// Supports legacy property syntax
/// </summary>
public class PropertiesConverter : JsonConverter<Dictionary<string, string>>
{
    protected class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonConverter(typeof(JsonStringConverter))]
        public string Value { get; set; }
    }

    public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var result = JsonSerializer.Deserialize<Property[]>(ref reader, options).ToDictionary(key => key.Name, value => value.Value.ToString());
            return result;
        }
        else
            return JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}