using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TiledLib.Layer;
using TiledLib.Objects;

namespace TiledLib
{
    static class TmxParsing
    {
        public static BaseObject ReadObject(this XmlReader reader)
        {
            if (!reader.IsStartElement("object"))
                throw new XmlException(reader.Name);

            var x = reader["x"].ParseInt32().Value;
            var y = reader["y"].ParseInt32().Value;
            var w = reader["width"].ParseInt32();
            var h = reader["height"].ParseInt32();
            var name = reader["name"];
            var type = reader["type"];

            BaseObject result = null;
            var properties = new Dictionary<string, string>();
            if (reader.IsEmptyElement)
                reader.Skip();
            else
            {
                reader.ReadStartElement("object");

                while (reader.IsStartElement())
                    switch (reader.Name)
                    {
                        case "properties":
                            reader.ReadProperties(properties);
                            break;
                        case "ellipse":
                            result = new EllipseObject(properties)
                            {
                                X = x,
                                Y = y,
                                Width = w.Value,
                                Height = h.Value,
                                Name = name,
                                IsEllipse = true,
                                ObjectType = type
                            };
                            reader.Skip();
                            break;
                        case "polygon":
                            result = new PolygonObject(properties)
                            {
                                X = x,
                                Y = y,
                                Name = name,
                                polygon = reader.ReadPoints().ToArray(),
                                ObjectType = type
                            };
                            reader.Skip();
                            break;
                        case "polyline":
                            result = new PolyLineObject(properties)
                            {
                                X = x,
                                Y = y,
                                Name = name,
                                polyline = reader.ReadPoints().ToArray(),
                                ObjectType = type
                            };
                            reader.Skip();
                            break;
                        default:
                            throw new XmlException(reader.Name);
                    }

                if (reader.Name == "object")
                    reader.ReadEndElement();
                else
                    throw new XmlException($"Expected </object>, found: {reader.Name}");
            }

            return result ?? new RectangleObject(properties)
            {
                X = x,
                Y = y,
                Width = w.Value,
                Height = h.Value,
                Name = name,
                ObjectType = type
            };
        }

        static IEnumerable<Position> ReadPoints(this XmlReader reader)
            => from p in reader["points"].Split(' ')
               let split = p.IndexOf(',')
               select new Position(int.Parse(p.Substring(0, split)), int.Parse(p.Substring(split + 1)));

        public static void ReadLayerAttributes(this XmlReader reader, BaseLayer layer)
        {
            layer.Name = reader["name"] ?? throw new KeyNotFoundException("name");
            if (!(layer is ObjectLayer))
            {
                layer.Width = reader["width"].ParseInt32() ?? throw new KeyNotFoundException("width");
                layer.Height = reader["height"].ParseInt32() ?? throw new KeyNotFoundException("height");

                layer.X = reader["x"].ParseInt32() ?? 0;
                layer.Y = reader["y"].ParseInt32() ?? 0;
            }

            layer.Opacity = reader["opacity"].ParseDouble() ?? 1.0;
            layer.Visible = reader["visible"].ParseBool() ?? true;
        }

        public static void ReadMapAttributes(this XmlReader reader, Map map)
        {
            map.Version = reader["version"];
            map.TiledVersion = reader["tiledversion"];
            map.Orientation = (Orientation)Enum.Parse(typeof(Orientation), reader["orientation"]);
            map.RenderOrder = (RenderOrder)Enum.Parse(typeof(RenderOrder), reader["renderorder"]?.Replace("-", ""));
            map.Width = int.Parse(reader["width"]);
            map.Height = int.Parse(reader["height"]);
            map.CellWidth = int.Parse(reader["tilewidth"]);
            map.CellHeight = int.Parse(reader["tileheight"]);
        }

        public static void ReadMapElements(this XmlReader reader, Map map)
        {
            var tilesets = new List<ITileset>();
            var layers = new List<BaseLayer>();
            reader.ReadStartElement("map");
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "tileset":
                        if (reader["source"] == null)
                        {
                            var xmlSerializer = new XmlSerializer(typeof(Tileset));
                            tilesets.Add((Tileset)xmlSerializer.Deserialize(reader));
                        }
                        else
                        {
                            tilesets.Add(new ExternalTileset { firstgid = int.Parse(reader["firstgid"]), source = reader["source"] });
                            reader.Read();
                        }
                        break;
                    case "layer":
                        var xmlSerializer1 = new XmlSerializer(typeof(TileLayer));
                        layers.Add((BaseLayer)xmlSerializer1.Deserialize(reader));
                        break;
                    case "objectgroup":
                        var xmlSerializer2 = new XmlSerializer(typeof(ObjectLayer));
                        layers.Add((BaseLayer)xmlSerializer2.Deserialize(reader));
                        break;
                    case "imagelayer":
                        var xmlSerializer3 = new XmlSerializer(typeof(ImageLayer));
                        layers.Add((BaseLayer)xmlSerializer3.Deserialize(reader));
                        break;
                    case "properties":
                        reader.ReadProperties(map.Properties);
                        break;
                    default:
                        throw new XmlException(reader.Name);
                }

            if (reader.Name == "map")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);

            map.Tilesets = tilesets.ToArray();
            map.Layers = layers.ToArray();
        }


        public static void ReadTileset(this XmlReader reader, Tileset ts)
        {
            if (!reader.IsStartElement("tileset"))
                throw new XmlException(reader.Name);

            if (reader["firstgid"] != null)
                ts.firstgid = int.Parse(reader["firstgid"]);

            ts.name = reader["name"];
            ts.tilewidth = int.Parse(reader["tilewidth"]);
            ts.tileheight = int.Parse(reader["tileheight"]);
            ts.spacing = int.Parse(reader["spacing"] ?? "0");

            var tileCount = reader["tilecount"].ParseInt32();
            var columns = reader["columns"].ParseInt32();

            reader.ReadStartElement("tileset");
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "image":
                        ts.ImagePath = reader["source"];
                        ts.imagewidth = int.Parse(reader["width"]);
                        ts.imageheight = int.Parse(reader["height"]);
                        reader.Skip();
                        break;
                    case "properties":
                        reader.ReadProperties(ts.Properties);
                        break;
                    case "tile":
                        reader.ReadTile(ts.TileProperties, ts.TileAnimations);
                        break;
                    default:
                        break;
                }

            if (reader.Name == "tileset")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);
        }

        static void ReadTile(this XmlReader reader, Dictionary<int, Dictionary<string, string>> tileProperties, Dictionary<int, Frame[]> tileAnimations)
        {
            if (!reader.IsStartElement("tile"))
                throw new XmlException(reader.Name);

            var id = int.Parse(reader["id"]);
            Dictionary<string, string> properties;
            if (!tileProperties.TryGetValue(id, out properties) || properties == null)
                properties = tileProperties[id] = new Dictionary<string, string>();

            reader.ReadStartElement("tile");
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "properties":
                        reader.ReadProperties(properties);
                        break;
                    case "animation":
                        tileAnimations[id] = reader.ReadAnimation();
                        break;
                    default:
                        reader.Skip(); //TODO: Add logging.
                        break;
                }

            if (reader.Name == "tile")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);
        }

        static Frame[] ReadAnimation(this XmlReader reader)
        {
            if (!reader.IsStartElement("animation"))
                throw new XmlException(reader.Name);

            var parent = XNode.ReadFrom(reader) as XElement;
            return parent
                .Elements()
                .Select(e => new Frame { TileId = int.Parse(e.Attribute("tileid").Value), Duration_ms = int.Parse(e.Attribute("duration").Value) })
                .ToArray();
        }

        public static void ReadProperties(this XmlReader reader, Dictionary<string, string> properties)
        {
            if (!reader.IsStartElement("properties"))
                throw new XmlException(reader.Name);

            var parent = XNode.ReadFrom(reader) as XElement;
            foreach (var e in parent.Elements())
                properties[e.Attribute("name").Value] = e.IsEmpty ? e.Attribute("value").Value : e.Value;
        }



        static int[] ReadCSV(this XmlReader reader, int size)
        {
            var data = reader.ReadElementContentAsString()
                .Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            if (data.Length == size)
                return data;
            else
                throw new XmlException();
        }

        static int[] ReadBase64(this XmlReader reader, int count)
        {
            var buffer = new byte[count * sizeof(int)];
            var size = reader.ReadElementContentAsBase64(buffer, 0, buffer.Length);
            if (reader.ReadElementContentAsBase64(buffer, 0, buffer.Length) != 0)
                throw new InvalidDataException();
            var data = new int[size / sizeof(int)];
            Buffer.BlockCopy(buffer, 0, data, 0, size);
            return data;
        }
        static int[] ReadBase64Decompress<T>(this XmlReader reader, Func<Stream, Zlib.CompressionMode, T> streamFactory, int size)
            where T : Stream
        {
            var buffer = new byte[size * sizeof(int)];

            var total = reader.ReadElementContentAsBase64(buffer, 0, buffer.Length);
            if (reader.ReadElementContentAsBase64(buffer, 0, buffer.Length) != 0)
                throw new InvalidDataException();

            using (var mstream = new MemoryStream(buffer, 0, total))
            using (var stream = streamFactory(mstream, Zlib.CompressionMode.Decompress))
            {
                var data = new int[size];
                var pos = 0;
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Buffer.BlockCopy(buffer, 0, data, pos, count);
                    pos += count;
                }
                return data;
            }
        }

        public static int[] ReadData(this XmlReader reader, int count, out string encoding, out string compression)
        {
            encoding = reader["encoding"];
            compression = reader["compression"];

            switch (encoding)
            {
                case "csv":
                    return reader.ReadCSV(count);
                case "base64":
                    switch (compression)
                    {
                        case null:
                            return reader.ReadBase64(count);
                        case "gzip":
                            return reader.ReadBase64Decompress((stream, mode) => new Zlib.GZipStream(stream, mode), count);
                        case "zlib":
                            return reader.ReadBase64Decompress((stream, mode) => new Zlib.ZlibStream(stream, mode), count);
                        default:
                            throw new XmlException(compression);
                    }
                default:
                    throw new NotImplementedException($"Encoding: {encoding}");
            }
        }

    }
}