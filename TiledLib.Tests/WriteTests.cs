using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Org.XmlUnit.Builder;
using Org.XmlUnit.Diff;


namespace TiledLib.Tests;

[TestClass]
public class WriteTests
{
    [TestMethod]
    [DataRow("Data/empty.tmx")]
    [DataRow("Data/Hexagonal_tileset.tmx")]
    [DataRow("Data/External_tileset_map.tmx")]
    [DataRow("Data/tileset_map_base64.tmx")]
    [DataRow("Data/External_tileset_map_base64.tmx")]
    [DataRow("Data/Multi_image_tileset_infinite_map_base64_zstd.tmx")]
    [DataRow("Data/tileset_map_base64_zstd_wangsets.tmx")]
    public void TestWriting(string file)
    {
        using var original = new MemoryStream();
        using (var mapStream = File.OpenRead(file))
        {
            mapStream.CopyTo(original);
            original.Seek(0, SeekOrigin.Begin);
        }

        var map = Map.FromStream(original);
        original.Seek(0, SeekOrigin.Begin);
        using var output = new MemoryStream();
        using (var writer = new StreamWriter(output, encoding: Encoding.UTF8, leaveOpen: true))
        {
            new XmlSerializer(typeof(Map)).Serialize(writer, map);
        }
        original.Seek(0, SeekOrigin.Begin);
        output.Seek(0, SeekOrigin.Begin);

        var expected = Encoding.UTF8.GetString(original.ToArray()).Replace("UTF-8", "utf-8");
        var result = Encoding.UTF8.GetString(output.ToArray()).TrimStart('\uFEFF');


        var xmlDiff = DiffBuilder.Compare(Input.FromString(expected))
            .WithTest(Input.FromString(result))
            .CheckForSimilar() // Similar handles different element/attribute ordering safely
            .WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndAllAttributes(a => a switch
            {
                { Name: "type", OwnerElement.Name: "property" } => false,
                { Name: "infinite", OwnerElement.Name: "map" } => false,
                _ => true
            })))
            //.WithNodeMatcher(new DefaultNodeMatcher(ElementSelectors.ByNameAndAllAttributes))
            .IgnoreWhitespace()
            .IgnoreElementContentWhitespace()
            .IgnoreComments()
            .WithDifferenceEvaluator((c, o) => o == ComparisonResult.DIFFERENT && c switch
            {
                // 1. Attribute count matches on a <property> element
                {
                    Type: ComparisonType.ELEMENT_NUM_ATTRIBUTES,
                    ControlDetails.Target.Name: "property" or "map"
                } => true,
                // 2. The 'type' attribute is missing on a <property> element
                {
                    ControlDetails.Target: XmlAttribute { Name: "type", OwnerElement.Name: "property" }
                } => true,
                // 3. The 'type' attribute value is different on a <property> element
                {
                    TestDetails.Target: XmlAttribute { Name: "type", OwnerElement.Name: "property" }
                } => true,
                {
                    Type: ComparisonType.ATTR_NAME_LOOKUP,
                    ControlDetails.Target.Name: "property",
                    ControlDetails.Value: XmlQualifiedName { Name: "type" }
                } => true,
                {
                    Type: ComparisonType.ATTR_NAME_LOOKUP,
                    ControlDetails.Target.Name: "map",
                    ControlDetails.Value: XmlQualifiedName { Name: "infinite" }
                } => true,
                {
                    Type: ComparisonType.ATTR_NAME_LOOKUP,
                    TestDetails.Target.Name: "map",
                    TestDetails.Value: XmlQualifiedName { Name: "infinite" }
                } => true,
                {
                    Type: ComparisonType.TEXT_VALUE,
                    ControlDetails.Target.ParentNode.Name: "chunk" or "data",
                } => Enumerable.SequenceEqual(Base64DecompressZstd(c.ControlDetails.Value.ToString()!).ToArray(), Base64DecompressZstd(c.TestDetails.Value.ToString()!).ToArray()),
                // Fallback for everything else
                _ => false
            } ? ComparisonResult.EQUAL : o)
            .WithDifferenceListeners((comparison, outcome) =>
            {
                //File.WriteAllText("expected.xml", expected);
                //File.WriteAllText("result.xml", result);
                Assert.Fail($"found a difference: {comparison}");
            })
            .Build();
    }

    static ReadOnlySpan<uint> Base64DecompressZstd(string input)
    {
        var ms = new MemoryStream(System.Convert.FromBase64String(input));
        ms.Seek(0, SeekOrigin.Begin);
        using var stream = new ZstdSharp.DecompressionStream(ms);
        using var ms_output = new MemoryStream();
        stream.CopyTo(ms_output);
        var bytes = ms_output.ToArray();
        var data = MemoryMarshal.Cast<byte, uint>(bytes);
        return data;
    }
}
