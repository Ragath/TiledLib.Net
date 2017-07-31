using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TiledLib.Layer;

namespace TiledLib.Tests
{
    [TestClass]
    public class LayerFormatTests
    {
        [DataTestMethod]
        [DataRow(@"Data\External_tileset_map.json")]
        [DataRow(@"Data\External_tileset_map.tmx")]
        [DataRow(@"Data\External_tileset_map_base64.json")]
        [DataRow(@"Data\External_tileset_map_base64.tmx")]
        [DataRow(@"Data\Level0.json")]
        public void TestUncompressedDecoding(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                var result = Map.FromStream(stream);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Layers);
                Assert.IsTrue(result.Layers.Any());
                foreach (var tl in result.Layers.OfType<TileLayer>())
                    Assert.IsFalse(tl.data.All(i => i == 0));
            }
        }

        [DataTestMethod]
        [DataRow(@"Data\External_tileset_map_base64_gzip.tmx", @"Data\External_tileset_map_base64.tmx")]
        [DataRow(@"Data\External_tileset_map_base64_zlib.tmx", @"Data\External_tileset_map_base64.tmx")]
        [DataRow(@"Data\External_tileset_map_base64_zlib.json", @"Data\External_tileset_map_base64.json")]
        [DataRow(@"Data\External_tileset_map_base64_gzip.json", @"Data\External_tileset_map_base64.json")]
        public void TestCompression(string file, string referenceFile)
        {
            Map referenceMap;
            using (var stream = File.OpenRead(referenceFile))
            {
                referenceMap = Map.FromStream(stream);
            }

            using (var stream = File.OpenRead(file))
            {
                var result = Map.FromStream(stream);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Layers);
                Assert.IsTrue(result.Layers.Any());
                foreach (var tl in result.Layers.OfType<TileLayer>())
                    Assert.IsFalse(tl.data.All(i => i == 0));

                Assert.AreEqual(referenceMap.Layers.Length, result.Layers.Length);
                for (int i = 0; i < referenceMap.Layers.Length; i++)
                    if (referenceMap.Layers[i] is TileLayer refLayer)
                    {
                        var layer = result.Layers[i] as TileLayer;
                        Assert.IsNotNull(layer);
                        Assert.IsNotNull(layer.Compression);
                        Assert.AreEqual("base64", layer.Encoding);
                        CollectionAssert.AreEqual(refLayer.data, layer.data);
                    }
            }
        }
    }
}