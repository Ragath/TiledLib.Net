using System;
using System.Collections.Generic;

namespace TiledLib
{
    public class ExternalTileset : ITileset
    {
        public string Source { get; set; }

        public int FirstGid { get; set; }

        private Lazy<Tileset> _Tileset { get; set; }
        ITileset Tileset => _Tileset.Value;

        public int Columns => Tileset.Columns;

        public int ImageHeight => Tileset.ImageHeight;
        public string ImagePath => System.IO.Path.IsPathRooted(Tileset.ImagePath) ? Tileset.ImagePath : System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Source), Tileset.ImagePath);
        public int ImageWidth => Tileset.ImageWidth;
        public int Margin => Tileset.Margin;
        public string Name => Tileset.Name;

        public int Rows => Tileset.Rows;
        public int Spacing => Tileset.Spacing;
        public int TileCount => Tileset.TileCount;

        public int TileHeight => Tileset.TileHeight;
        public int TileWidth => Tileset.TileWidth;

        public string TransparentColor => Tileset.TransparentColor;

        public TileOffset TileOffset => Tileset.TileOffset;

        public Dictionary<string, string> Properties => Tileset.Properties;
        public Dictionary<int, Dictionary<string, string>> TileProperties => Tileset.TileProperties;

        public Tile this[int gid] => Tileset[gid];
        public string this[int gid, string property] => Tileset[gid, property];

        public void LoadTileset()
        {
            var v = _Tileset.Value;
        }
        public void LoadTileset(Func<ExternalTileset, Tileset> loader)
        {
            _Tileset = new Lazy<Tileset>(() =>
            {
                var tileset = loader(this);
                tileset.FirstGid = this.FirstGid;
                return tileset;
            });
            LoadTileset();
        }
    }
}