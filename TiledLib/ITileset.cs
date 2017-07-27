using System.Collections.Generic;
using Newtonsoft.Json;

namespace TiledLib
{
    [JsonConverter(typeof(TilesetConverter))]
    public interface ITileset
    {
        Tile this[int id] { get; }

        int Columns { get; }
        int firstgid { get; }
        int imageheight { get; }
        string ImagePath { get; }
        int imagewidth { get; }
        int margin { get; }
        string name { get; }

        Dictionary<string, string> Properties { get; }
        int Rows { get; }
        int spacing { get; }
        int TileCount { get; }
        int tileheight { get; }

        Dictionary<int, Dictionary<string, string>> TileProperties { get; }
        int tilewidth { get; }
        string transparentcolor { get; }
    }
}