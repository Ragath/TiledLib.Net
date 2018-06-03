using Newtonsoft.Json;

namespace TiledLib
{

    public static class Utils
    {
        internal static readonly JsonSerializer JsonSerializer = new JsonSerializer();
        internal static readonly System.Xml.Serialization.XmlSerializer XmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Map));

        public static Map ReadJsonMap(this System.IO.TextReader reader) => (Map)JsonSerializer.Deserialize(reader, typeof(Map));
        public static Map ReadTmxMap(this System.IO.TextReader reader) => (Map)XmlSerializer.Deserialize(reader);

        public static void WriteTmxMap(this System.IO.TextWriter writer, Map map) => XmlSerializer.Serialize(writer, map);

        static long GetPosition(this System.IO.StreamReader reader) => reader.BaseStream.Position;
        static void SetPosition(this System.IO.StreamReader reader, long position)
        {
            reader.BaseStream.Position = position;
            reader.DiscardBufferedData();
        }

        internal static bool ContainsJson(this System.IO.StreamReader reader)
        {
            var startPosition = reader.GetPosition();
            for (char c = (char)reader.Read(); c != '{'; c = (char)reader.Read())
                if (c != '\r' && c != '\n' && !char.IsWhiteSpace(c))
                {
                    reader.SetPosition(startPosition);
                    return false;
                }

            reader.SetPosition(startPosition);
            return true;
        }
    }
}