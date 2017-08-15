using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using TiledLib.Layer;

namespace TiledLib.Tests
{
    [TestClass]
    public class TileTests
    {
        [DataTestMethod]
        [DataRow("Data/External_tileset_map.tmx")]
        [DataRow("Data/External_tileset_map.json")]
        [DataRow("Data/External_tileset_map.tmx")]
        [DataRow("Data/External_tileset_map_base64.json")]
        [DataRow("Data/External_tileset_map_base64.tmx")]
        [DataRow("Data/External_tileset_map_base64_gzip.tmx")]
        [DataRow("Data/External_tileset_map_base64_zlib.tmx")]
        [DataRow("Data/External_tileset_map_base64_zlib.json")]
        [DataRow("Data/External_tileset_map_base64_gzip.json")]
        [DataRow("Data/Level0.json")]
        public void TestTileIndexer(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), ts.source)));

                foreach (var layer in map.Layers.OfType<TileLayer>())
                {
                    for (int y = 0, i = 0; y < layer.Height; y++)
                        for (int x = 0; x < layer.Width; x++, i++)
                        {
                            var gid = layer.data[i];
                            if (gid == 0)
                                continue;

                            var tileset = map.Tilesets.Single(ts => gid >= ts.firstgid && ts.firstgid + ts.TileCount > gid);
                            var tile = tileset[gid];

                            Assert.AreNotEqual(0, tile.Width);
                            Assert.AreNotEqual(0, tile.Height);
                        }
                }
            }
        }
    }
}
