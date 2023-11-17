# TiledLib.Net

[![Build Status](https://dev.azure.com/RagathGithub/TiledLib/_apis/build/status/TiledLib-CI?branchName=master)](https://dev.azure.com/RagathGithub/TiledLib/_build/latest?definitionId=1&branchName=master)  
Cross-platform Tiled map parsing utilities.

## Nugets

[![](Docs/Images/nuget.png) TiledLib](https://www.nuget.org/packages/TiledLib/) - Core library, everything you need to read Tiled maps/tilesets.  
[![](Docs/Images/nuget.png) TiledLib.Pipeline](https://www.nuget.org/packages/TiledLib.Pipeline/) - MonoGame content pipeline extension, provides a ContentImporter for Tiled maps. You can add a processor yourself to map the import to your own classes

## Basic usecase

```csharp
 using (var stream = File.OpenRead(filename))
 using (var stream = File.OpenRead(filename))
  {
    var map = Map.FromStream(stream, ts => File.OpenRead(Path.Combine(Path.GetDirectoryName(filename), ts.Source)));

    foreach (var layer in map.Layers.OfType<TileLayer>())
    {
      for (int y = 0, i = 0; y < layer.Height; y++)
          for (int x = 0; x < layer.Width; x++, i++)
          {
            var gid = layer.data[i];
            if (gid == 0)
              continue;

            var tileset = map.Tilesets.Single(ts => gid >= ts.FirstGid && ts.FirstGid + ts.TileCount > gid);
            var tile = tileset[gid];

              // Do stuff with the tile.
          }
    }
  }
```
