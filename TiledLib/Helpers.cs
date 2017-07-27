using System.Globalization;

namespace TiledLib
{
    static class Helpers
    {
        static bool[] translate = new[] { false, true };
        public static int? ParseInt32(this string str) => str == null ? (int?)null : int.Parse(str, CultureInfo.InvariantCulture);
        public static double? ParseDouble(this string str) => str == null ? (double?)null : double.Parse(str, CultureInfo.InvariantCulture);
        public static bool? ParseBool(this string str) => str == null ? (bool?)null : translate[int.Parse(str, CultureInfo.InvariantCulture)];
    }
}
