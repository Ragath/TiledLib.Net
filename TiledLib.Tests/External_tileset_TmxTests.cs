using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TiledLib.Tests
{
    [TestClass]
    public class ParseTests
    {
        [DataTestMethod]
        [DataRow("Data/External_tileset_map.tmx")]
        [DataRow("Data/Hexagonal_tileset.tmx")]
        public void TestTmxParsing(string file)
        {
            using (var mapStream = File.OpenRead(file))
            {
                var result = new XmlSerializer(typeof(Map)).Deserialize(mapStream) as Map;

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Layers);
                Assert.IsTrue(result.Layers.Any());
            }
        }

        [DataTestMethod]
        [DataRow("Data/Level0.json")]
        [DataRow("Data/Hexagonal_tileset.json")]
        public void TestJsonParsing(string file)
        {
            var mapData = File.ReadAllText(file);
            var result = JsonConvert.DeserializeObject<Map>(mapData);
            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow("Data/External_tileset.json")]
        [DataRow("Data/External_tileset.tsx")]
        public void TestWangParsing(string file)
        {
            using (var f = new FileStream(file, FileMode.Open))
            {
                var tileset = Tileset.FromStream(f);
                Assert.AreEqual(1, tileset.WangSets.Length);
                var set = tileset.WangSets[0];
                Assert.AreEqual(3, set.CornerColors.Length);
                Assert.IsTrue(set.WangTiles.Length > 0);
                var grass = set.WangTiles.Single(x => x.TileId == 0);
                grass.CompressedWangId = grass.CompressedWangId;
                CollectionAssert.AreEqual(new[] { 0, 2, 0, 2, 0, 2, 0, 2 }, grass.WangId);
            }
        }
    }
}
