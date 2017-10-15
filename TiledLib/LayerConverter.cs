using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TiledLib.Layer;

namespace TiledLib
{
    public class LayerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(BaseLayer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            byte[] buffer = null;
            BaseLayer result;
            switch (jo["type"].Value<string>())
            {
                case "tilelayer":
                    result = new TileLayer();
                    if (jo["encoding"]?.Value<string>() == "base64")
                    {
                        buffer = Convert.FromBase64String(jo["data"].Value<string>());
                        jo["data"].Replace(JToken.FromObject(new int[0]));
                    }
                    break;
                case "objectgroup":
                    result = new ObjectLayer();
                    break;
                case "imagelayer":
                    result = new ImageLayer();
                    break;
                default:
                    return null;
            }
            serializer.Populate(jo.CreateReader(), result);

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
                            using (var stream = new Zlib.ZlibStream(mStream, Zlib.CompressionMode.Decompress))
                            {
                                var bufferSize = result.Width * result.Height * sizeof(int);
                                Array.Resize(ref buffer, bufferSize);
                                stream.Read(buffer, 0, bufferSize);

                                if (stream.ReadByte() != -1)
                                    throw new JsonException();

                                tl.Data = new int[result.Width * result.Height];
                                Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                            }
                        }
                        break;
                    case "gzip":
                        using (var mStream = new MemoryStream(buffer))
                        {
                            using (var stream = new Zlib.GZipStream(mStream, Zlib.CompressionMode.Decompress))
                            {
                                var bufferSize = result.Width * result.Height * sizeof(int);
                                Array.Resize(ref buffer, bufferSize);
                                stream.Read(buffer, 0, bufferSize);

                                if (stream.ReadByte() != -1)
                                    throw new JsonException();

                                tl.Data = new int[result.Width * result.Height];
                                Buffer.BlockCopy(buffer, 0, tl.Data, 0, buffer.Length);
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Compression: {tl.Compression}");
                }


            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
