namespace EmojiNet;

/// <summary>
/// Represents the shortcodes information of an <see cref="Emoji"/>.
/// </summary>
public sealed class EmojiShortcodes
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmojiShortcodes"/> class.
    /// </summary>
    /// <param name="emoji">The emoji to create the list for.</param>
    internal EmojiShortcodes(Emoji emoji)
    {
        Emoji = emoji;

        var langs = EmojiShortcodesDatabase.LanguageNames;
        var dbs = EmojiShortcodesDatabase.DatabaseNames;

        var byLang = new Dictionary<string, Dictionary<string, IReadOnlyList<string>>>();
        var byDb = new Dictionary<string, Dictionary<string, IReadOnlyList<string>>>();

        foreach (var lang in langs)
        {
            byLang[lang] = new Dictionary<string, IReadOnlyList<string>>();
        }

        foreach (var db in dbs)
        {
            byDb[db] = new Dictionary<string, IReadOnlyList<string>>();
        }

        ByLanguageThenByDatabase = byLang.AsReadOnly(
            static x => x.Key,
            static x => x.Value.AsReadOnly());
        ByDatabaseThenByLanguage = byDb.AsReadOnly(
            static x => x.Key,
            static x => x.Value.AsReadOnly());
    }

    /// <summary>
    /// The emoji that corresponds with this set of shortcodes.
    /// </summary>
    public Emoji Emoji { get; }

    /// <summary>
    /// Accesses the shortcodes on a by-language-then-by-database fashion.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> ByLanguageThenByDatabase { get; }

    /// <summary>
    /// Accesses the shortcodes on a by-database-then-by-language fashion.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> ByDatabaseThenByLanguage { get; }
}
