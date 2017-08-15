# TiledLib.Net
[![Build status](https://ci.appveyor.com/api/projects/status/qiygwb08oa0r90rm/branch/master?svg=true)](https://ci.appveyor.com/project/Ragath/tiledlib-net/branch/master)  
Cross-platform Tiled map parsing utilities.

## Nugets
[![](Docs/Images/nuget.png) TiledLib](https://www.nuget.org/packages/TiledLib/) - Core library, everything you need to read Tiled maps/tilesets.  
[![](Docs/Images/nuget.png) TiledLib.Pipeline](https://www.nuget.org/packages/TiledLib.Pipeline/) - MonoGame content pipeline extension, provides a ContentImporter for Tiled maps.  

## Basic usecase
```csharp
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

        // Do stuff with the tile.
      }
  }
}
```
