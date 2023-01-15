using System.Text.Json.Nodes;
using TiledLib.Layer;

namespace TiledLib;

public class LayerConverter : JsonConverter<BaseLayer>
{
    public override BaseLayer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jo = JsonElement.ParseValue(ref reader);
        byte[] buffer = null;
        BaseLayer result;
        switch (jo.GetProperty("type").GetString())
        {
            case "tilelayer":
                var jObject = JsonObject.Create(jo);
                if (jObject["encoding"]?.GetValue<string>() == "base64")
                {
                    buffer = Convert.FromBase64String(jObject["data"].GetValue<string>());
                    jObject.Remove("data");
                }
                result = jObject.Deserialize<TileLayer>(options);
                break;
            case "objectgroup":
                result = jo.Deserialize<ObjectLayer>(options);
                break;
            case "imagelayer":
                result = jo.Deserialize<ImageLayer>(options);
                break;
            default:
                return null;
        }

        if (result is TileLayer tl && buffer != null)
            switch (tl.Compression)
            {
                case null:
                    tl.Data = new int[buffer.Length / sizeof(int)];
                    Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                    break;
                case "zlib":
                    using (var mStream = new MemoryStream(buffer))
                    {
                        using var stream = new Zlib.ZlibStream(mStream, Zlib.CompressionMode.Decompress);
                        var bufferSize = result.Width * result.Height * sizeof(int);
                        Array.Resize(ref buffer, bufferSize);
                        stream.Read(buffer, 0, bufferSize);

                        if (stream.ReadByte() != -1)
                            throw new JsonException();

                        tl.Data = new int[result.Width * result.Height];
                        Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                    }
                    break;
                case "gzip":
                    using (var mStream = new MemoryStream(buffer))
                    {
                        using var stream = new Zlib.GZipStream(mStream, Zlib.CompressionMode.Decompress);
                        var bufferSize = result.Width * result.Height * sizeof(int);
                        Array.Resize(ref buffer, bufferSize);
                        stream.Read(buffer, 0, bufferSize);

                        if (stream.ReadByte() != -1)
                            throw new JsonException();

                        tl.Data = new int[result.Width * result.Height];
                        Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                    }
                    break;
                default:
                    throw new NotImplementedException($"Compression: {tl.Compression}");
            }


        return result;
    }

    public override void Write(Utf8JsonWriter writer, BaseLayer value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
