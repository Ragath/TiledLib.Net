﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TiledLib.Layer;

namespace TiledLib
{
    static class TmxMisc
    {
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

        public static void ReadTileset(this XmlReader reader, Tileset ts)
        {
            if (!reader.IsStartElement("tileset"))
                throw new XmlException(reader.Name);

            ts.Version = reader["version"];
            ts.TiledVersion = reader["tiledversion"];

            if (reader["firstgid"] != null)
                ts.FirstGid = int.Parse(reader["firstgid"]);

            ts.Name = reader["name"];
            ts.TileWidth = int.Parse(reader["tilewidth"]);
            ts.TileHeight = int.Parse(reader["tileheight"]);
            ts.Spacing = int.Parse(reader["spacing"] ?? "0");

            var tileCount = reader["tilecount"].ParseInt32();
            var columns = reader["columns"].ParseInt32();

            reader.ReadStartElement("tileset");
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case "tileoffset":
                        ts.TileOffset = new TileOffset();
                        ts.TileOffset.X = int.Parse(reader["x"]);
                        ts.TileOffset.Y = int.Parse(reader["y"]);
                        reader.Skip();
                        break;
                    case "image":
                        ts.ImagePath = reader["source"];
                        ts.ImageWidth = int.Parse(reader["width"]);
                        ts.ImageHeight = int.Parse(reader["height"]);
                        reader.Skip();
                        break;
                    case "properties":
                        reader.ReadProperties(ts.Properties);
                        break;
                    case "tile":
                        reader.ReadTile(ts.TileProperties, ts.TileAnimations);
                        break;
                    case "wangsets":
                        reader.ReadStartElement();
                        ts.WangSets = reader.ReadArray<WangSet>("wangset");
                        reader.ReadEndElement();
                        break;
                    default:
                        reader.Skip();
                        break;
                }

            if (reader.Name == "tileset")
                reader.ReadEndElement();
            else
                throw new XmlException(reader.Name);
        }

        public static void WriteTileset(this XmlWriter writer, Tileset ts, bool isExternal)
        {
            if (isExternal)
            {
                if(ts.Version != null)
                    writer.WriteAttribute("version", ts.Version);
                if(ts.TiledVersion != null)
                    writer.WriteAttribute("tiledversion", ts.TiledVersion);
            }
            else
            {
                writer.WriteAttribute("firstgid", ts.FirstGid);
            }
            if (ts.Name != null)
                writer.WriteAttribute("name", ts.Name);

            writer.WriteAttribute("tilewidth", ts.TileWidth);
            writer.WriteAttribute("tileheight", ts.TileHeight);
            if (ts.Spacing != 0)
                writer.WriteAttribute("spacing", ts.Spacing);
            if (ts.TileCount != 0)
                writer.WriteAttribute("tilecount", ts.TileCount);
            if (ts.Columns != 0)
                writer.WriteAttribute("columns", ts.Columns);

            if (ts.TileOffset != null)
            {
                writer.WriteStartElement("tileoffset");
                writer.WriteAttribute("x", ts.TileOffset.X);
                writer.WriteAttribute("y", ts.TileOffset.Y);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("image");
            {
                writer.WriteAttribute("source", ts.ImagePath);
                if (ts.ImageWidth != 0)
                    writer.WriteAttribute("width", ts.ImageWidth);
                if (ts.ImageHeight != 0)
                    writer.WriteAttribute("height", ts.ImageHeight);
            }
            writer.WriteEndElement();

            writer.WriteProperties(ts.Properties);

            foreach (var t in ts.TileProperties)
            {
                writer.WriteStartElement("tile");
                {
                    writer.WriteAttribute("id", t.Key);

                    writer.WriteProperties(t.Value);
                    if (ts.TileAnimations != null)
                        if (ts.TileAnimations.TryGetValue(t.Key, out var anim) && anim?.Length > 0)
                            writer.WriteAnimation(anim);
                }
                writer.WriteEndElement();
            }

            if (ts.WangSets != null)
            {
                writer.WriteStartElement("wangsets");
                writer.WriteArray("wangset", ts.WangSets);
                writer.WriteEndElement();
            }
        }

        static void ReadTile(this XmlReader reader, Dictionary<int, Dictionary<string, string>> tileProperties, Dictionary<int, Frame[]> tileAnimations)
        {
            if (!reader.IsStartElement("tile"))
                throw new XmlException(reader.Name);

            var id = int.Parse(reader["id"]);
            Dictionary<string, string> properties;
            if (!tileProperties.TryGetValue(id, out properties) || properties == null)
                properties = tileProperties[id] = new Dictionary<string, string>();

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement("tile");
            }
            else
            {
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

        static void WriteAnimation(this XmlWriter writer, Frame[] animation)
        {
            writer.WriteStartElement("animation");
            foreach (var frame in animation)
            {
                writer.WriteStartElement("frame");
                writer.WriteAttribute("tileid", frame.TileId);
                writer.WriteAttribute("duration", frame.Duration_ms);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
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

        public static void WriteAttribute(this XmlWriter writer, string localName, int? value)
        {
            if (value == null)
                return;
            writer.WriteAttribute(localName, value.Value);
        }


        public static void WriteAttribute(this XmlWriter writer, string localName, Orientation value)
        {
            switch (value)
            {
                case Orientation.unknown:
                    break;
                case Orientation.orthogonal:
                    writer.WriteAttributeString(localName, "orthogonal");
                    break;
                case Orientation.isometric:
                    writer.WriteAttributeString(localName, "isometric");
                    break;
                case Orientation.hexagonal:
                    writer.WriteAttributeString(localName, "hexagonal");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, RenderOrder value)
        {
            switch (value)
            {
                case RenderOrder.Unknown:
                    break;
                case RenderOrder.rightdown:
                    writer.WriteAttributeString(localName, "right-down");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, StaggerAxis value)
        {
            if (value == StaggerAxis.None)
                return;
            switch (value)
            {
                case StaggerAxis.x:
                    writer.WriteAttributeString(localName, "x");
                    break;
                case StaggerAxis.y:
                    writer.WriteAttributeString(localName, "y");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, StaggerIndex value)
        {
            if (value == StaggerIndex.None)
                return;
            switch (value)
            {
                case StaggerIndex.odd:
                    writer.WriteAttributeString(localName, "odd");
                    break;
                case StaggerIndex.even:
                    writer.WriteAttributeString(localName, "even");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public static void WriteAttribute(this XmlWriter writer, string localName, int value)
            => writer.WriteAttributeString(localName, value.ToString(CultureInfo.InvariantCulture));

        public static void WriteAttribute(this XmlWriter writer, string localName, double value)
            => writer.WriteAttributeString(localName, value.ToString("0.##", CultureInfo.InvariantCulture));

        public static void WriteAttribute(this XmlWriter writer, string localName, bool value)
            => writer.WriteAttributeString(localName, value ? "1" : "0");

        public static void WriteAttribute(this XmlWriter writer, string localName, string value)
            => writer.WriteAttributeString(localName, value);

        public static T[] ReadArray<T>(this XmlReader reader, string elementName)
        {
            var s = new XmlSerializer(typeof(T), new XmlRootAttribute(elementName));
            var results = new List<T>();
            while (reader.IsStartElement())
            {
                results.Add((T)s.Deserialize(reader));
            }
            return results.ToArray();
        }

        public static void WriteArray<T>(this XmlWriter writer, string elementName, T[] values)
        {
            var s = new XmlSerializer(typeof(T), new XmlRootAttribute(elementName));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            foreach (var value in values)
            {
                s.Serialize(writer, value, ns);
            }
        }


    }
}