using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TiledLib.Layer;
using TiledLib.Objects;

namespace TiledLib
{
    static class TmxParsing
    {
        public static IEnumerable<BaseObject> ReadObjectLayerElements(this XmlReader reader)
        {
            if (reader.ReadToDescendant("object"))
                do
                {
                    var x = reader["x"].ParseInt32().Value;
                    var y = reader["y"].ParseInt32().Value;
                    var w = reader["width"].ParseInt32();
                    var h = reader["height"].ParseInt32();
                    var name = reader["name"];

                    if (reader.IsEmptyElement)
                        yield return new RectangleObject
                        {
                            X = x,
                            Y = y,
                            Width = w.Value,
                            Height = h.Value,
                            Name = name
                        };
                    else
                    {
                        reader.Read();
                        if (!reader.IsEmptyElement)
                            throw new XmlException();

                        switch (reader.Name)
                        {
                            case "ellipse":
                                yield return new EllipseObject
                                {
                                    X = x,
                                    Y = y,
                                    Width = w.Value,
                                    Height = h.Value,
                                    Name = name,
                                    IsEllipse = true
                                };
                                reader.Read();
                                break;
                            case "polygon":
                                var pts = from p in reader["points"].Split(' ')
                                          let split = p.IndexOf(',')
                                          select new Position(int.Parse(p.Substring(0, split)), int.Parse(p.Substring(split + 1)));

                                yield return new PolygonObject
                                {
                                    X = x,
                                    Y = y,
                                    Name = name,
                                    polygon = pts.ToArray()
                                    //Width = w,
                                    //Height = h,
                                };
                                reader.Read();
                                break;
                            default:
                                throw new XmlException();
                        }
                    }
                }
                while (reader.ReadToNextSibling("object"));
        }

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
        //------------Map-----------//
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
            while (reader.Read())
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "tileset":
                            if (reader["source"] == null)
                            {
                                var xmlSerializer = new XmlSerializer(typeof(Tileset));
                                tilesets.Add((Tileset)xmlSerializer.Deserialize(reader));
                            }
                            else
                                tilesets.Add(new ExternalTileset { firstgid = int.Parse(reader["firstgid"]), source = reader["source"] });
                            //        
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
                        default:
                            throw new XmlException(reader.Name);
                    }

                }

            map.Tilesets = tilesets.ToArray();
            map.Layers = layers.ToArray();
        }

        //------------Map-----------//
        public static void ReadTileset(this XmlReader reader, Tileset ts)
        {
            ts.name = reader["name"];
            ts.tilewidth = int.Parse(reader["tilewidth"]);
            ts.tileheight = int.Parse(reader["tilewidth"]);
            ts.spacing = int.Parse(reader["spacing"] ?? "0");

            var tileCount = int.Parse(reader["tilecount"]);
            var columns = int.Parse(reader["columns"]);



            if (!reader.ReadToDescendant("image"))
                throw new XmlException();
            else
            {
                ts.ImagePath = reader["source"];
                ts.imagewidth = int.Parse(reader["width"]);
                ts.imageheight = int.Parse(reader["height"]);
            }
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

        static int[] ReadBase64(this XmlReader reader, int size)
        {
            var buffer = new byte[size * 4];
            var count = reader.ReadElementContentAsBase64(buffer, 0, buffer.Length);
            if (reader.ReadElementContentAsBase64(buffer, 0, buffer.Length) != 0)
                throw new InvalidDataException();
            var data = new int[count / 4];
            Buffer.BlockCopy(buffer, 0, data, 0, count);
            return data;
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
                            throw new NotImplementedException("gzip");
                        case "zlib":
                            throw new NotImplementedException("zlib");
                        default:
                            throw new XmlException(compression);
                    }
                default:
                    throw new NotImplementedException();
            }
        }

    }
}