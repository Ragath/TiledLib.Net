using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using TiledLib.Layer;

namespace TiledLib
{
    static class TmxMap
    {
        public static void ReadMapAttributes(this XmlReader reader, Map map)
        {
            map.Version = reader["version"];
            map.TiledVersion = reader["tiledversion"];
            map.Orientation = (Orientation)Enum.Parse(typeof(Orientation), reader["orientation"]);
            map.RenderOrder = (RenderOrder)Enum.Parse(typeof(RenderOrder), reader["renderorder"]?.Replace("-", ""));
            map.StaggerAxis = reader["staggeraxis"] == null ? StaggerAxis.None : (StaggerAxis)Enum.Parse(typeof(StaggerAxis), reader["staggeraxis"]);
            map.StaggerIndex = reader["staggerindex"] == null ? StaggerIndex.None : (StaggerIndex)Enum.Parse(typeof(StaggerIndex), reader["staggerindex"]);
            map.Width = int.Parse(reader["width"]);
            map.Height = int.Parse(reader["height"]);
            map.CellWidth = int.Parse(reader["tilewidth"]);
            map.CellHeight = int.Parse(reader["tileheight"]);
            map.HexSideLength = reader["hexsidelength"] == null ? default(int?) : int.Parse(reader["hexsidelength"]);

            map.NextObjectId = reader["nextobjectid"].ParseInt32() ?? 0;
            map.BackgroundColor = reader["backgroundcolor"];
        }

        public static void WriteMapAttributes(this XmlWriter writer, Map map)
        {
            writer.WriteAttribute("version", map.Version);
            writer.WriteAttribute("tiledversion", map.TiledVersion);

            writer.WriteAttribute("orientation", map.Orientation);
            writer.WriteAttribute("renderorder", map.RenderOrder);
            writer.WriteAttribute("width", map.Width);
            writer.WriteAttribute("height", map.Height);
            writer.WriteAttribute("tilewidth", map.CellWidth);
            writer.WriteAttribute("tileheight", map.CellHeight);
            writer.WriteAttribute("hexsidelength", map.HexSideLength);
            writer.WriteAttribute("staggeraxis", map.StaggerAxis);
            writer.WriteAttribute("staggerindex", map.StaggerIndex);
            if (map.BackgroundColor != null)
                writer.WriteAttribute("backgroundcolor", map.BackgroundColor);

            if (map.NextObjectId != 0)
                writer.WriteAttribute("nextobjectid", map.NextObjectId);
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
                            tilesets.Add(new ExternalTileset { FirstGid = int.Parse(reader["firstgid"]), Source = reader["source"] });
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

        public static void WriteMapElements(this XmlWriter writer, Map map)
        {
            writer.WriteProperties(map.Properties);
            foreach (var tileset in map.Tilesets)
                switch (tileset)
                {
                    case ExternalTileset ts:
                        writer.WriteStartElement("tileset");
                        writer.WriteAttribute("firstgid", ts.FirstGid);
                        writer.WriteAttribute("source", ts.Source);
                        writer.WriteEndElement();
                        break;
                    case Tileset ts:
                        writer.WriteStartElement("tileset");
                        {
                            if (ts.FirstGid < 1)
                                throw new ArgumentOutOfRangeException(nameof(Tileset.FirstGid));
                            writer.WriteAttribute("firstgid", ts.FirstGid);
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

                            if (ts.Grid != null)
                            {
                                writer.WriteStartElement("grid");
                                writer.WriteAttribute("orientation", ts.Grid.Orientation);
                                writer.WriteAttribute("width", ts.Grid.Width);
                                writer.WriteAttribute("height", ts.Grid.Height);
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
                        }
                        writer.WriteEndElement();
                        break;
                    default:
                        throw new NotImplementedException();
                }

            foreach (var layer in map.Layers)
                switch (layer)
                {
                    case TileLayer l:
                        writer.WriteTileLayer(l);
                        break;
                    case ObjectLayer l:
                        writer.WriteObjectLayer(l);
                        break;
                    case ImageLayer l:
                        writer.WriteStartElement("imagelayer");
                        {
                            writer.WriteAttribute("name", l.Name);
                            if (!l.Visible)
                                writer.WriteAttribute("visible", l.Visible);
                            if (l.Opacity != 1)
                                writer.WriteAttribute("opacity", l.Opacity);


                            writer.WriteStartElement("image");
                            {
                                writer.WriteAttribute("source", l.Image);
                                // <image source="sewer_tileset.png"/> 
                            }
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        break;
                    default:
                        throw new NotImplementedException();
                }
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
    }
}