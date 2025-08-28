namespace EmojiNet.SourceGenerator;

/// <summary>
/// Generates the emoji databases.
/// </summary>
[Generator]
public sealed class EmojiDatabaseGenerator : IIncrementalGenerator
{
    private static readonly Regex FileNameRegex = new Regex(
        @"EmojiNet\.SourceGenerator\.Resources\.Shortcodes\.(?<lang>\w+)\.(?<source>\w+)\.json",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Generate);
    }

    private static void Generate(IncrementalGeneratorPostInitializationContext ctx)
    {
        ctx.AddSource("EmojiShortcodes.cs", Generate().Trim());
    }

    private static Dictionary<string, string[]> Parse(string content)
    {
        var result = new Dictionary<string, string[]>();

        foreach (var line in content.Split(['\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            var columns = trimmed.Split(['\t'], StringSplitOptions.RemoveEmptyEntries);

            if (columns.Length <= 1)
            {
                continue;
            }

            var name = columns[0];
            var values = new string[columns.Length - 1];
            Array.Copy(columns, 1, values, 0, values.Length);

            result[name] = values;
        }

        return result;
    }

    private static string Generate()
    {
        var asm = typeof(EmojiDatabaseGenerator).Assembly;
        var files = asm.GetManifestResourceNames();

        var data = new Dictionary<string, Dictionary<string, Dictionary<string, string[]>>>();

        foreach (var file in files)
        {
            var match = FileNameRegex.Match(file);
            if (!match.Success)
            {
                continue;
            }

            var lang = match.Groups["lang"].Value;
            var source = match.Groups["source"].Value;

            if (!data.TryGetValue(lang, out var langData))
            {
                langData = new();
                data[lang] = langData;
            }

            using var stream = asm.GetManifestResourceStream(file);
            using var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            var dataset = Parse(content);

            langData[source] = dataset;
        }

        var createDictionary = new StringBuilder();
        var prefix = new string(' ', 8);

        createDictionary.AppendLine($"{prefix}var dbTemp = new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>>();");

        foreach (var langEntry in data)
        {
            var lang = langEntry.Key;
            var langData = langEntry.Value;

            createDictionary.AppendLine($"{prefix}dbTemp[\"{lang}\"] = new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>();");

            foreach (var sourceEntry in langData)
            {
                var source = sourceEntry.Key;
                var sourceData = sourceEntry.Value;

                createDictionary.AppendLine($"{prefix}dbTemp[\"{lang}\"][\"{source}\"] = new Dictionary<string, IReadOnlyList<string>>();");

                foreach (var codepointEntry in sourceData)
                {
                    var codepoint = codepointEntry.Key;
                    var names = codepointEntry.Value;
                    var quotedNames = names.Select(static n => $"\"{n}\"");
                    var value = $"[{string.Join(", ", quotedNames)}]";

                    createDictionary.AppendLine($"{prefix}dbTemp[\"{lang}\"][\"{source}\"][\"{codepoint}\"] = {value};");
                }
            }
        }

        createDictionary.AppendLine($"{prefix}db = dbTemp.ToReadOnly();");

        return $$"""
namespace EmojiNet;

partial class EmojiShortcodes
{
    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>> db;

    static EmojiShortcodes()
    {
{{createDictionary}}
    }
}
""";
    }
}
