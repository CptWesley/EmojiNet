namespace EmojiNet;

/// <summary>
/// Represents the shortcodes information of an <see cref="Emoji"/>.
/// </summary>
public sealed class EmojiShortcodes
{
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>> byLanguageThenByDatabase;
    private readonly Lazy<IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>> byDatabaseThenByLanguage;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmojiShortcodes"/> class.
    /// </summary>
    /// <param name="emoji">The emoji to create the list for.</param>
    internal EmojiShortcodes(Emoji emoji)
    {
        Emoji = emoji;

        byLanguageThenByDatabase = new(() =>
        {
            var langs = EmojiShortcodesDatabase.LanguageNames;
            var dbs = EmojiShortcodesDatabase.DatabaseNames;

            var byLang = new Dictionary<string, Dictionary<string, IReadOnlyList<string>>>();

            foreach (var lang in langs)
            {
                var byDb = new Dictionary<string, IReadOnlyList<string>>();
                byLang[lang] = byDb;

                foreach (var db in dbs)
                {
                    byDb[db] = Array.Empty<string>();
                }
            }

            return byLang.AsReadOnly(
                static x => x.Key,
                static x => x.Value.AsReadOnly());
        });

        byDatabaseThenByLanguage = new(() =>
        {
            var langs = EmojiShortcodesDatabase.LanguageNames;
            var dbs = EmojiShortcodesDatabase.DatabaseNames;

            var byDb = new Dictionary<string, Dictionary<string, IReadOnlyList<string>>>();

            foreach (var db in dbs)
            {
                var byLang = new Dictionary<string, IReadOnlyList<string>>();
                byDb[db] = byLang;

                foreach (var lang in langs)
                {
                    byLang[db] = Array.Empty<string>();
                }
            }

            return byDb.AsReadOnly(
                static x => x.Key,
                static x => x.Value.AsReadOnly());
        });
    }

    /// <summary>
    /// The emoji that corresponds with this set of shortcodes.
    /// </summary>
    public Emoji Emoji { get; }

    /// <summary>
    /// Accesses the shortcodes on a by-language-then-by-database fashion.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> ByLanguageThenByDatabase => byLanguageThenByDatabase.Value;

    /// <summary>
    /// Accesses the shortcodes on a by-database-then-by-language fashion.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> ByDatabaseThenByLanguage => byDatabaseThenByLanguage.Value;
}
