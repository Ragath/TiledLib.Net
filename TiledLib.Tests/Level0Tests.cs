using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace TiledLib.Tests
{
    [TestClass]
    public class Level0Tests
    {
        const string Filename = @"Data\Level0.json";
        
        [TestMethod]
        public void TestJsonParsing()
        {
            var mapData = File.ReadAllText(Filename);
            var result = JsonConvert.DeserializeObject<Map>(mapData);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestTileIndexing()
        {
            var mapData = File.ReadAllText(Filename);
            var map = JsonConvert.DeserializeObject<Map>(mapData);

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
