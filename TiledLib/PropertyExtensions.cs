using System.Collections.Generic;

namespace TiledLib
{
    public static class PropertyExtensions
    {
        public static string GetValue(this Dictionary<string, string> properties, string key)
            => properties?.ContainsKey(key) == true ? properties[key] : null;
    }
}
