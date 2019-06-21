using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledLib.Pipeline
{
    [ContentImporter(new[] { ".tmx", ".json" }, CacheImportedData = false, DisplayName = "Tiled Map Importer", DefaultProcessor = "PassThroughProcessor")]
    public class TiledMapImporter : ContentImporter<Map>
    {
        public override Map Import(string filename, ContentImporterContext context)
        {
            string GetPath(string path) => Path.IsPathRooted(path) ? path : Path.Combine(Path.GetDirectoryName(filename), path);

            using (var stream = File.OpenRead(filename))
            {
                var map = Map.FromStream(stream, ts => File.OpenRead(GetPath(ts.Source)));

                foreach (var ts in map.Tilesets.OfType<ExternalTileset>())
                    ts.LoadTileset();

                return map;
            }
        }
    }
}
