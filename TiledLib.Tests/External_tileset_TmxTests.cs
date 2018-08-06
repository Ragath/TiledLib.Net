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
    }
}
