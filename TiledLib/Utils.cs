namespace TiledLib;


public static class Utils
{
    internal static readonly System.Xml.Serialization.XmlSerializer XmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Map));

    public static Map ReadJsonMap(this Stream stream) => JsonSerializer.Deserialize<Map>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    public static Map ReadTmxMap(this TextReader reader) => (Map)XmlSerializer.Deserialize(reader);

    public static void WriteTmxMap(this TextWriter writer, Map map) => XmlSerializer.Serialize(writer, map);

    static long GetPosition(this StreamReader reader) => reader.BaseStream.Position;
    static void SetPosition(this StreamReader reader, long position)
    {
        reader.BaseStream.Position = position;
        reader.DiscardBufferedData();
    }

    internal static bool ContainsJson(this Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        return ContainsJson(reader);
    }
    internal static bool ContainsJson(this StreamReader reader)
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

    public static int GetId(int gid) => gid & (int)TileOrientation.MaskID;
    public static TileOrientation GetOrientation(int gid) => (TileOrientation)gid & TileOrientation.MaskFlip;
}