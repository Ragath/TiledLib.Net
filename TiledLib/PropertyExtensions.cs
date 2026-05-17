namespace TiledLib;

public static class PropertyExtensions
{
    [Obsolete("Use GetValueOrDefault instead.")]
    public static string? GetValue(this Dictionary<string, string> properties, string key)
        => properties?.GetValueOrDefault(key);
}
