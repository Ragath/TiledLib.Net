namespace TiledLib;

public class ExternalTileset : ITileset
{
    public string? Source { get; set; }

    public int FirstGid { get; set; }

    private Lazy<Tileset>? _Tileset { get; set; }
    ITileset Tileset => _Tileset!.Value;

    public int Columns => Tileset.Columns;

    public string? ImagePath => Tileset.ImagePath switch
    {
        null => null,
        var path when System.IO.Path.IsPathRooted(path) => path,
        var path => System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Source)!, path)
    };
    public int ImageHeight => Tileset.ImageHeight;
    public int ImageWidth => Tileset.ImageWidth;
    public int Margin => Tileset.Margin;
    public string Name => Tileset.Name;

    public int Rows => Tileset.Rows;
    public int Spacing => Tileset.Spacing;
    public int TileCount => Tileset.TileCount;

    public int TileHeight => Tileset.TileHeight;
    public int TileWidth => Tileset.TileWidth;

    public string? TransparentColor => Tileset.TransparentColor;

    public TileOffset? TileOffset => Tileset.TileOffset;

    public Dictionary<string, string> Properties => Tileset.Properties;
    public Dictionary<int, Dictionary<string, string>> TileProperties => Tileset.TileProperties;
    public Dictionary<int, Frame[]> TileAnimations => Tileset.TileAnimations;

    public Tile this[uint gid] => Tileset[gid];
    public string? this[uint gid, string property] => Tileset[gid, property];

    public void LoadTileset()
    {
        _ = _Tileset!.Value;
    }
    public void LoadTileset(Func<ExternalTileset, Tileset> loader)
    {
        _Tileset = new Lazy<Tileset>(() =>
        {
            var tileset = loader(this);
            tileset.FirstGid = this.FirstGid & (int)TileOrientation.MaskID;
            return tileset;
        });
        LoadTileset();
    }
}