using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TiledLib.Tests
{
    [TestClass]
    public class External_tileset_TmxTests
    {
        const string Filename = @"Data\External_tileset_map.tmx";

        [TestMethod]
        public void TestTmxParsing()
        {
            using (var mapStream = File.OpenRead(Filename))
            {
                var result = Map.FromStream(mapStream);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Layers);
                Assert.IsTrue(result.Layers.Any());
            }
        }

        [TestMethod]
        public void TestTileIndexing()
        {
            using (var mapStream = File.OpenRead(Filename))
            {
                var result = Map.FromStream(mapStream);
                Assert.IsNotNull(result);


                Tileset LoadTileset(ExternalTileset t)
                {
                    using (var stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(Filename), t.source)))
                    {
                        return Tileset.FromStream(stream);
                    }
                }

                foreach (var ts in result.Tilesets.OfType<ExternalTileset>())
                    ts.LoadTileset(LoadTileset);
                var q = from t in result.Tilesets
                        from i in Enumerable.Range(t.firstgid, t.Rows * t.Columns)
                        select new
                        {
                            index = i,
                            tile = t[i]
                        };
                CollectionAssert.AllItemsAreUnique(q.ToArray().Select(i => i.index).ToArray());
            }
        }
    }
}
