﻿using System.Globalization;
using System.Text;
using System.Xml;
using TiledLib.Layer;
using TiledLib.Objects;

namespace TiledLib;

static class TmxLayers
{
    public static void WriteTileLayer(this XmlWriter writer, TileLayer layer)
    {
        writer.WriteStartElement("layer");
        {
            if (layer.Id != default)
                writer.WriteAttribute("id", layer.Id);
            writer.WriteAttribute("name", layer.Name);
            writer.WriteAttribute("width", layer.Width);
            writer.WriteAttribute("height", layer.Height);
            if (!layer.Visible)
                writer.WriteAttribute("visible", layer.Visible);
            if (layer.Opacity != 1)
                writer.WriteAttribute("opacity", layer.Opacity);

            if (layer.OffsetX != default)
                writer.WriteAttribute("offsetx", layer.OffsetX);
            if (layer.OffsetY != default)
                writer.WriteAttribute("offsety", layer.OffsetY);

            writer.WriteProperties(layer.Properties);

            writer.WriteStartElement("data");
            {
                writer.WriteAttribute("encoding", layer.Encoding);
                //TODO: writer.WriteAttribute("compression", layer.Compression);
                switch (layer.Encoding)
                {
                    case "csv":
                        writer.WriteCSV(layer);
                        break;
                    case "base64":
                        writer.WriteBase64(layer);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    public static void WriteObjectLayer(this XmlWriter writer, ObjectLayer layer)
    {
        writer.WriteStartElement("objectgroup");

        if (layer.Id != default)
            writer.WriteAttribute("id", layer.Id);
        writer.WriteAttribute("name", layer.Name);
        if (!layer.Visible)
            writer.WriteAttribute("visible", layer.Visible);
        if (layer.Opacity != 1)
            writer.WriteAttribute("opacity", layer.Opacity);

        if (layer.OffsetX != default)
            writer.WriteAttribute("offsetx", layer.OffsetX);
        if (layer.OffsetY != default)
            writer.WriteAttribute("offsety", layer.OffsetY);

        writer.WriteProperties(layer.Properties);
        foreach (var o in layer.Objects)
            writer.WriteObject(o);

        writer.WriteEndElement();
    }


    static void WriteCSV(this XmlWriter writer, TileLayer layer)
    {
        writer.WriteRaw(Environment.NewLine);
        var sb = new StringBuilder(layer.Width * 4);
        for (int y = 0, i = 0; y < layer.Height; y++)
        {
            sb.Clear();
            for (int x = 0; x < layer.Width; x++, i++)
            {
                sb.Append(layer.Data[i]);
                sb.Append(',');
            }
            if (y + 1 == layer.Height)
                sb.Length--;
            sb.AppendLine();
            writer.WriteRaw(sb.ToString());
        }
    }

    static void WriteBase64(this XmlWriter writer, TileLayer layer)
    {
        var buffer = new byte[layer.Data.Length * sizeof(int)];
        Buffer.BlockCopy(layer.Data, 0, buffer, 0, buffer.Length);

        writer.WriteRaw(Environment.NewLine);
        writer.WriteBase64(buffer, 0, buffer.Length);
        writer.WriteRaw(Environment.NewLine);
    }


    public static BaseObject ReadObject(this XmlReader reader)
    {
        if (!reader.IsStartElement("object"))
            throw new XmlException(reader.Name);

        var id = reader["id"].ParseInt32() ?? 0;

        var name = reader["name"];
        var type = reader["type"];

        var gid = reader["gid"].ParseInt32();

        var x = reader["x"].ParseDouble().Value;
        var y = reader["y"].ParseDouble().Value;
        var w = reader["width"].ParseDouble();
        var h = reader["height"].ParseDouble();

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
                            Id = id,
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
                            Id = id,
                            X = x,
                            Y = y,
                            Name = name,
                            Polygon = reader.ReadPoints().ToArray(),
                            ObjectType = type
                        };
                        reader.Skip();
                        break;
                    case "polyline":
                        result = new PolyLineObject(properties)
                        {
                            Id = id,
                            X = x,
                            Y = y,
                            Name = name,
                            Polyline = reader.ReadPoints().ToArray(),
                            ObjectType = type
                        };
                        reader.Skip();
                        break;
                    case "point":
                        result = new PointObject(properties)
                        {
                            Id = id,
                            X = x,
                            Y = y,
                            Name = name,
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

        if (gid.HasValue)
            result = new TileObject(properties)
            {
                Id = id,
                Gid = gid.Value,
                X = x,
                Y = y,
                Width = w.Value,
                Height = h.Value,
                Name = name,
                ObjectType = type
            };

        return result ?? new RectangleObject(properties)
        {
            Id = id,
            X = x,
            Y = y,
            Width = w ?? 0,
            Height = h ?? 0,
            Name = name,
            ObjectType = type
        };
    }

    public static void WriteObject(this XmlWriter writer, BaseObject entity)
    {
        writer.WriteStartElement("object");
        if (entity.Id != 0)
            writer.WriteAttribute("id", entity.Id);

        if (entity is TileObject t)
            writer.WriteAttribute("gid", t.Gid);

        if (entity.Name != null)
            writer.WriteAttribute("name", entity.Name);

        if (entity.ObjectType != null)
            writer.WriteAttribute("type", entity.ObjectType);

        writer.WriteAttribute("x", entity.X);
        writer.WriteAttribute("y", entity.Y);

        void HW()
        {
            writer.WriteAttribute("width", entity.Width);
            writer.WriteAttribute("height", entity.Height);
        }


        switch (entity)
        {
            case EllipseObject _:
                HW();
                writer.WriteProperties(entity.Properties);

                writer.WriteStartElement("ellipse");
                writer.WriteEndElement();
                break;
            case RectangleObject _:
                HW();
                writer.WriteProperties(entity.Properties);

                break;
            case TileObject _:
                HW();
                writer.WriteProperties(entity.Properties);

                break;
            case PolygonObject o:
                writer.WriteProperties(entity.Properties);

                writer.WriteStartElement("polygon");
                writer.WritePoints(o.Polygon);
                writer.WriteEndElement();
                break;
            case PolyLineObject o:
                writer.WriteProperties(entity.Properties);

                writer.WriteStartElement("polyline");
                writer.WritePoints(o.Polyline);
                writer.WriteEndElement();
                break;
            case PointObject _:
                writer.WriteProperties(entity.Properties);

                writer.WriteStartElement("point");
                writer.WriteEndElement();
                break;
            default:
                throw new NotImplementedException();
        }

        writer.WriteEndElement();
    }


    static IEnumerable<Position> ReadPoints(this XmlReader reader)
        => from p in reader["points"].Split(' ')
           let split = p.IndexOf(',')
           select new Position(double.Parse(p.Substring(0, split), CultureInfo.InvariantCulture), double.Parse(p.Substring(split + 1), CultureInfo.InvariantCulture));

    static void WritePoints(this XmlWriter writer, Position[] points)
    {
        if (points.Length == 0)
            return;

        var sb = new StringBuilder(points.Length * 8);
        foreach (var p in points)
        {
            sb.Append(p.X.ToString("0.##", CultureInfo.InvariantCulture));
            sb.Append(',');
            sb.Append(p.Y.ToString("0.##", CultureInfo.InvariantCulture));
            sb.Append(' ');
        }
        sb.Length--;

        writer.WriteAttribute("points", sb.ToString());
    }
}