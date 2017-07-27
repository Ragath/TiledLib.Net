using System;
using System.Collections.Generic;

namespace TiledLib
{

    public class ExternalTileset : ITileset
    {
        public string source { get; set; }

        public int firstgid { get; set; }

        internal Lazy<Tileset> _Tileset { get; set; }
        ITileset Tileset => _Tileset.Value;

        public int Columns => Tileset.Columns;

        public int imageheight => Tileset.imageheight;
        public string ImagePath => Tileset.ImagePath;
        public int imagewidth => Tileset.imagewidth;
        public int margin => Tileset.margin;
        public string name => Tileset.name;

        public Dictionary<string, string> Properties => Tileset.Properties;

        public int Rows => Tileset.Rows;

        public int spacing => Tileset.spacing;

        public int TileCount => Tileset.TileCount;

        public int tileheight => Tileset.tileheight;

        public Dictionary<int, Dictionary<string, string>> TileProperties => Tileset.TileProperties;

        public int tilewidth => Tileset.tilewidth;
        public string transparentcolor => Tileset.transparentcolor;        

        public Tile this[int id] => Tileset[id];

        public void LoadTileset()
        {
            var v = _Tileset.Value;
        }
        public void LoadTileset(Func<ExternalTileset, Tileset> loader)
        {
            _Tileset = new Lazy<Tileset>(() =>
            {
                var tileset = loader(this);
                tileset.firstgid = this.firstgid;
                return tileset;
            });
            LoadTileset();
        }
    }
}