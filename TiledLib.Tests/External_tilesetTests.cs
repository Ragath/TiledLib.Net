using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TiledLib.Tests
{
    [TestClass]
    public class External_tilesetTests
    {
        const string Filename = @"Data\External_tileset_map.json";
        
        [TestMethod]
        public void TestJsonParsing()
        {
            using (var stream = File.OpenRead(Filename))
            {
                var result = Map.FromStream(stream);
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void TestTileIndexing()
        {
            using (var mapStream = File.OpenRead(Filename))
            {
                var map = Map.FromStream(mapStream);

                Tileset LoadTileset(ExternalTileset t)
                {
                    using (var stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(Filename), t.source)))
                    {
                        return Tileset.FromStream(stream);
                    }
                }

                foreach (var ts in map.Tilesets.OfType<ExternalTileset>())
                    ts.LoadTileset(LoadTileset);
                var q = from t in map.Tilesets
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
