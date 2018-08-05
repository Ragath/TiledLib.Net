using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib
{
    [JsonConverter(typeof(TilesetConverter))]
    public interface ITileset
    {
        Tile this[int gid] { get; }
        string this[int gid, string property] { get; }

        int Columns { get; }
        int FirstGid { get; }
        int ImageHeight { get; }
        string ImagePath { get; }
        int ImageWidth { get; }
        int Margin { get; }
        string Name { get; }

        Dictionary<string, string> Properties { get; }
        int Rows { get; }
        int Spacing { get; }
        int TileCount { get; }
        int TileHeight { get; }
        int TileWidth { get; }

        Dictionary<int, Dictionary<string, string>> TileProperties { get; }
        string TransparentColor { get; }

        TileOffset TileOffset { get; }
    }
}