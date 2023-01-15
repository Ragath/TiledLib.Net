namespace TiledLib.Objects;

class ObjectConverter : JsonConverter<BaseObject>
{
    public override BaseObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jo = JsonElement.ParseValue(ref reader);

        return jo switch
        {
            var a when a.TryGetProperty("gid", out var _) => a.Deserialize<TileObject>(options),
            var a when a.TryGetProperty("polygon", out var _) => a.Deserialize<PolygonObject>(options),
            var a when a.TryGetProperty("polyline", out var _) => a.Deserialize<PolyLineObject>(options),
            var a when a.TryGetProperty("ellipse", out var _) => a.Deserialize<EllipseObject>(options),
            _ => jo.Deserialize<RectangleObject>(options)
        };
    }

    public override void Write(Utf8JsonWriter writer, BaseObject value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
