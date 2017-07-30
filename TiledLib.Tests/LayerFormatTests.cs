using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TiledLib.Tests
{
    [TestClass]
    public class LayerFormatTests
    {
        static void ValidateResult(Map result)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Layers);
            Assert.IsTrue(result.Layers.Any());
        }

        [TestMethod]
        public void TestBase64Tmx()
        {
            using (var mapStream = File.OpenRead(@"Data\External_tileset_map_base64.tmx"))
            {
                var result = Map.FromStream(mapStream);
                ValidateResult(result);
            }
        }

        [TestMethod]
        public void TestBase64Json()
        {
            using (var mapStream = File.OpenRead(@"Data\External_tileset_map_base64.json"))
            {
                var result = Map.FromStream(mapStream);
                ValidateResult(result);
            }
        }
    }
}
