namespace TiledLib;

public readonly struct Position
{
    [JsonPropertyName("x")]
    public readonly double X;
    [JsonPropertyName("y")]
    public readonly double Y;

    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }
}
