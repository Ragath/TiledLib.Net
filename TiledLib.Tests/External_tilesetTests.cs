using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TiledLib.Tests
{
    [TestClass]
    public class TilesetTests
    {
        [DataTestMethod]
        [DataRow("Data/External_tileset_map.json")]
        [DataRow("Data/External_tileset_map.tmx")]
        [DataRow("Data/Level0.json")]
        public void TestTileIndexing(string file)
        {
            using (var mapStream = File.OpenRead(file))
            {
                var map = Map.FromStream(mapStream);

                Tileset LoadTileset(ExternalTileset t)
                {
                    using (var stream = File.OpenRead(Path.Combine(Path.GetDirectoryName(file), t.Source)))
                    {
                        return Tileset.FromStream(stream);
                    }
                }

                foreach (var ts in map.Tilesets.OfType<ExternalTileset>())
                    ts.LoadTileset(LoadTileset);
                var q = from t in map.Tilesets
                        from i in Enumerable.Range(t.FirstGid, t.Rows * t.Columns)
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
