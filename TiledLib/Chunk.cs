namespace TiledLib;

public class Chunk
{
    public required int X { get; set; }
    public required int Y { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public uint[] Data { get; set; } = [];
}
