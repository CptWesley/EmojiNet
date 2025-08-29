namespace EmojiNet;

/// <summary>
/// Used to resolve shortcodes to emojis.
/// </summary>
public sealed class EmojiShortcodeResolver : IReadOnlyDictionary<string, Emoji>
{
    private readonly IReadOnlyDictionary<string, Emoji> dict;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmojiShortcodeResolver"/> class.
    /// </summary>
    /// <param name="languages">The languages to resolve from.</param>
    /// <param name="databases">The databases to resolve from.</param>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
    internal EmojiShortcodeResolver(string[] languages, string[] databases)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
    {
        LanguageNames = languages.AsReadOnly();
        DatabaseNames = databases.AsReadOnly();

        var temp = new Dictionary<string, Emoji>();

        for (var li = languages.Length - 1; li >= 0; li--)
        {
            var lang = languages[li];

            if (!EmojiShortcodesDatabase.PerLanguage.TryGetValue(lang, out var langData))
            {
                continue;
            }

            for (var di = databases.Length - 1; di >= 0; di--)
            {
                var db = databases[di];

                if (!langData.TryGetValue(db, out var dbData))
                {
                    continue;
                }

                foreach (var entry in dbData)
                {
                    var codepoints = entry.Key;
                    var shortCodes = entry.Value;

                    var sb = new StringBuilder();
                    foreach (var cp in codepoints)
                    {
                        var c = char.ConvertFromUtf32(cp);
                        sb.Append(c);
                    }

                    var str = sb.ToString();
                    var emoji = Emojis.Lookup[str];

                    foreach (var shortCode in shortCodes)
                    {
                        temp[shortCode] = emoji;
                    }
                }
            }
        }

        dict = temp.AsReadOnly();
    }

    /// <inheritdoc />
    public Emoji this[string key] => dict[key];

    /// <inheritdoc />
    public IEnumerable<string> Keys => dict.Keys;

    /// <inheritdoc />
    public IEnumerable<Emoji> Values => dict.Values;

    /// <summary>
    /// Gets the languages used by this resolver.
    /// </summary>
    public IReadOnlyList<string> LanguageNames { get; }

    /// <summary>
    /// Gets the databases used by this resolver.
    /// </summary>
    public IReadOnlyList<string> DatabaseNames { get; }

    /// <inheritdoc />
    public int Count => dict.Count;

    /// <inheritdoc />
    public bool ContainsKey(string key)
        => dict.ContainsKey(key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Emoji>> GetEnumerator()
        => dict.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Emoji value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        => dict.TryGetValue(key, out value);
}
