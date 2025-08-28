namespace EmojiNet.Internal;

/// <summary>
/// Contains various databases of emoji shortcodes.
/// </summary>
internal static partial class EmojiShortcodesDatabase
{
    private const string ResourceNameRegexPattern = @"EmojiNet\.Resources\.Shortcodes\.(?<lang>[\w\-]+)\.(?<source>[\w\-]+)\.json";

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>>> perLanguage;
    private static readonly Lazy<IReadOnlyList<string>> languages;
    private static readonly Lazy<IReadOnlyList<string>> databases;

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
#pragma warning disable S3963 // "static" fields should be initialized inline
    static EmojiShortcodesDatabase()
#pragma warning restore S3963 // "static" fields should be initialized inline
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
    {
        perLanguage = new(() =>
        {
            var asm = typeof(EmojiShortcodesDatabase).Assembly;
            var resourceNames = asm.GetManifestResourceNames();

            var result = new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>>();

            foreach (var resourceName in resourceNames)
            {
                var match = ResourceNameRegex.Match(resourceName);

                if (!match.Success)
                {
                    continue;
                }

                var lang = match.Groups["lang"].Value.Trim();
                var source = match.Groups["source"].Value.Trim();

                if (!result.TryGetValue(lang, out var langData))
                {
                    langData = new Dictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>();
                    result[lang] = langData;
                }

                var sourceData = new Dictionary<IReadOnlyList<int>, IReadOnlyList<string>>();

                using var stream = asm.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream!);

                while (reader.ReadLine() is { } line)
                {
                    var trimmed = line.Trim();
                    var columns = trimmed.Split(['\t'], StringSplitOptions.RemoveEmptyEntries);

                    if (columns.Length <= 1)
                    {
                        continue;
                    }

                    var codepointsAsString = columns[0];
                    var codepoints = codepointsAsString
                        .Split(['-'], StringSplitOptions.RemoveEmptyEntries)
                        .Select(static hex => int.Parse(hex, NumberStyles.HexNumber))
                        .ToArray()
                        .AsReadOnly();
                    var values = new string[columns.Length - 1];
                    Array.Copy(columns, 1, values, 0, values.Length);

                    sourceData[codepoints] = values.AsReadOnly();
                }

                ((Dictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>)langData)[source] = sourceData.AsReadOnly();
            }

            foreach (var langEntry in result)
            {
                var lang = langEntry.Key;
                var langData = (Dictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>)langEntry.Value;

                result[lang] = langData.AsReadOnly();
            }

            return result.AsReadOnly();
        });

        languages = new(() => perLanguage.Value.Keys.OrderBy(static x => x).ToArray().AsReadOnly());
        databases = new(() => perLanguage.Value.Values.SelectMany(static x => x.Keys).Distinct().OrderBy(static x => x).ToArray().AsReadOnly());
    }

    /// <summary>
    /// Gets the data in a per-language-per-database way.
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<IReadOnlyList<int>, IReadOnlyList<string>>>> PerLanguage => perLanguage.Value;

    /// <summary>
    /// Gets the names of all the available languages.
    /// </summary>
    public static IReadOnlyList<string> LanguageNames => languages.Value;

    /// <summary>
    /// Gets the names of all the available databases.
    /// </summary>
    public static IReadOnlyList<string> DatabaseNames => databases.Value;

#if NET7_0_OR_GREATER
    [GeneratedRegex(ResourceNameRegexPattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex CreateResourceNameRegex();

    private static Regex ResourceNameRegex { get; } = CreateResourceNameRegex();
#else
    private static Regex ResourceNameRegex { get; } = new Regex(ResourceNameRegexPattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
#endif
}
