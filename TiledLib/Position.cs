using Newtonsoft.Json;

namespace TiledLib
{
    public struct Position
    {
        [JsonProperty("x")]
        public readonly double X;
        [JsonProperty("y")]
        public readonly double Y;

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
