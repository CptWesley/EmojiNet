using System.IO.Compression;

namespace EmojiNet.Internal;

/// <summary>
/// Contains various databases of emoji shortcodes.
/// </summary>
internal static partial class EmojiShortcodesDatabase
{
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
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
            var resourceName = "EmojiNet.Resources.Shortcodes.brotli.bin";
#else
            var resourceName = "EmojiNet.Resources.Shortcodes.gzip.bin";
#endif

            using var stream = asm.GetManifestResourceStream(resourceName);
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
            using var decompressionStream = new BrotliStream(stream!, CompressionMode.Decompress);
#else
            using var decompressionStream = new GZipStream(stream!, CompressionMode.Decompress);
#endif
            using var reader = new StreamReader(decompressionStream);

            var builder = new Dictionary<string, Dictionary<string, Dictionary<IReadOnlyList<int>, IReadOnlyList<string>>>>();
            Dictionary<IReadOnlyList<int>, IReadOnlyList<string>>? data = null;

            while (reader.ReadLine() is { } line)
            {
                var trimmed = line.Trim();
                var columns = trimmed.Split(['\t'], StringSplitOptions.RemoveEmptyEntries);

                if (columns.Length <= 1)
                {
                    continue;
                }

                if (columns.Length == 3 && columns[0] == "=")
                {
                    var lang = columns[1];
                    var source = columns[2];

                    if (!builder.TryGetValue(lang, out var langData))
                    {
                        langData = new();
                        builder[lang] = langData;
                    }

                    if (!langData.TryGetValue(source, out var sourceData))
                    {
                        sourceData = new();
                        langData[source] = sourceData;
                    }

                    data = sourceData;
                    continue;
                }
                else if (data is null)
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

                data[codepoints] = values.AsReadOnly();
            }

            var result = builder
                .AsReadOnly(
                    static x => x.Key,
                    static x => x.Value
                        .AsReadOnly(
                            static x => x.Key,
                            static x => x.Value.AsReadOnly()));

            return result;
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
}
