using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CurveEditor.Curves;

/// <summary>
/// Reads and writes curves as XNA content XML, one curve per file, the same format as
/// KatanaEngine's Curve::LoadFromFile / SaveToFile, XNA and MonoGame:
/// <code>
/// &lt;XnaContent xmlns:Framework="Microsoft.Xna.Framework"&gt;
///   &lt;Asset Type="Framework:Curve"&gt;
///     &lt;PreLoop&gt;Constant&lt;/PreLoop&gt;
///     &lt;PostLoop&gt;Constant&lt;/PostLoop&gt;
///     &lt;Keys&gt;
///       &lt;CurveKey&gt;&lt;Position&gt;0&lt;/Position&gt;&lt;Value&gt;1&lt;/Value&gt;&lt;TangentIn&gt;0&lt;/TangentIn&gt;&lt;TangentOut&gt;0&lt;/TangentOut&gt;&lt;Continuity&gt;Smooth&lt;/Continuity&gt;&lt;/CurveKey&gt;
///     &lt;/Keys&gt;
///   &lt;/Asset&gt;
/// &lt;/XnaContent&gt;
/// </code>
/// Keys may also be in XNA's compact form ("position value tangentIn tangentOut continuity"
/// for each key, separated by spaces). Files are written in the expanded form.
/// </summary>
public static class CurveFile
{
    public sealed record Contents(CurveLoopType PreLoop, CurveLoopType PostLoop, List<EditCurveKey> Keys);

    /// <exception cref="InvalidDataException">The file isn't a curve.</exception>
    public static Contents Load(string path)
    {
        XDocument document;
        try
        {
            document = XDocument.Load(path);
        }
        catch (XmlException ex)
        {
            throw new InvalidDataException($"The file isn't valid XML: {ex.Message}", ex);
        }

        var asset = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "Asset" && IsCurveType((string?)element.Attribute("Type")))
            ?? throw new InvalidDataException("The file has no curve (an <Asset Type=\"Framework:Curve\"> element).");

        var preLoop = ParseLoop(Child(asset, "PreLoop"));
        var postLoop = ParseLoop(Child(asset, "PostLoop"));

        var keysElement = asset.Elements().FirstOrDefault(element => element.Name.LocalName == "Keys");
        var keys = keysElement == null ? []
            : keysElement.HasElements ? ParseExpandedKeys(keysElement) : ParseCompactKeys(keysElement.Value);

        return new Contents(preLoop, postLoop, keys.OrderBy(key => key.Position).ToList());
    }

    public static void Save(string path, EditCurve curve)
    {
        var xml = new StringBuilder();
        xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n")
            .Append("<XnaContent xmlns:Framework=\"Microsoft.Xna.Framework\">\r\n")
            .Append("  <Asset Type=\"Framework:Curve\">\r\n")
            .Append($"    <PreLoop>{curve.PreLoop}</PreLoop>\r\n")
            .Append($"    <PostLoop>{curve.PostLoop}</PostLoop>\r\n")
            .Append("    <Keys>\r\n");

        foreach (var key in curve.Keys)
        {
            xml.Append("      <CurveKey>\r\n")
                .Append($"        <Position>{Format(key.Position)}</Position>\r\n")
                .Append($"        <Value>{Format(key.Value)}</Value>\r\n")
                .Append($"        <TangentIn>{Format(key.TangentIn)}</TangentIn>\r\n")
                .Append($"        <TangentOut>{Format(key.TangentOut)}</TangentOut>\r\n")
                .Append($"        <Continuity>{key.Continuity}</Continuity>\r\n")
                .Append("      </CurveKey>\r\n");
        }

        xml.Append("    </Keys>\r\n")
            .Append("  </Asset>\r\n")
            .Append("</XnaContent>\r\n");

        File.WriteAllText(path, xml.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    /// <summary>The shortest text that reads back as the same float.</summary>
    public static string Format(float value) => value.ToString("R", CultureInfo.InvariantCulture);

    // "Framework:Curve", "Microsoft.Xna.Framework.Curve" or plain "Curve".
    private static bool IsCurveType(string? type) =>
        type != null && type[(type.LastIndexOfAny([':', '.']) + 1)..] == "Curve";

    private static string? Child(XElement parent, string name) =>
        parent.Elements().FirstOrDefault(element => element.Name.LocalName == name)?.Value;

    private static CurveLoopType ParseLoop(string? text) =>
        text == null ? CurveLoopType.Constant
        : Enum.TryParse<CurveLoopType>(text.Trim(), ignoreCase: true, out var loop) && Enum.IsDefined(loop) ? loop
        : throw new InvalidDataException($"Unknown loop type \"{text.Trim()}\".");

    private static CurveContinuity ParseContinuity(string? text) =>
        text == null ? CurveContinuity.Smooth
        : Enum.TryParse<CurveContinuity>(text.Trim(), ignoreCase: true, out var continuity) && Enum.IsDefined(continuity) ? continuity
        : throw new InvalidDataException($"Unknown continuity \"{text.Trim()}\".");

    private static float ParseFloat(string? text, string name) =>
        float.TryParse(text?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value
        : throw new InvalidDataException($"Bad or missing {name} \"{text}\".");

    private static List<EditCurveKey> ParseExpandedKeys(XElement keys) =>
        keys.Elements().Where(element => element.Name.LocalName == "CurveKey").Select(element => new EditCurveKey
        {
            Position = ParseFloat(Child(element, "Position"), "Position"),
            Value = ParseFloat(Child(element, "Value"), "Value"),
            TangentIn = Child(element, "TangentIn") is { } tangentIn ? ParseFloat(tangentIn, "TangentIn") : 0f,
            TangentOut = Child(element, "TangentOut") is { } tangentOut ? ParseFloat(tangentOut, "TangentOut") : 0f,
            Continuity = ParseContinuity(Child(element, "Continuity")),
        }).ToList();

    private static List<EditCurveKey> ParseCompactKeys(string text)
    {
        var tokens = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length % 5 != 0) throw new InvalidDataException("The compact key list doesn't have 5 entries per key.");

        return Enumerable.Range(0, tokens.Length / 5).Select(index => new EditCurveKey
        {
            Position = ParseFloat(tokens[index * 5], "Position"),
            Value = ParseFloat(tokens[index * 5 + 1], "Value"),
            TangentIn = ParseFloat(tokens[index * 5 + 2], "TangentIn"),
            TangentOut = ParseFloat(tokens[index * 5 + 3], "TangentOut"),
            Continuity = ParseContinuity(tokens[index * 5 + 4]),
        }).ToList();
    }
}
