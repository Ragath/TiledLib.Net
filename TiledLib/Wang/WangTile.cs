namespace TiledLib.Wang;

public class WangTile
{
    [JsonPropertyName("tileid")]
    public int TileId { get; set; }

    /// <summary>
    /// 8 Wang color indexes (uchar[8]), 0 = unset, 1 = first color.
    /// Order: top, top-right, right, bottom-right, bottom, bottom-left, left, top-left
    /// </summary>
    [JsonPropertyName("wangid")]
    public int[] WangIds { get; set; } = new int[8];
}
