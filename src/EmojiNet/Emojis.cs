namespace EmojiNet;

/// <summary>
/// Provides a database of all known emojis.
/// </summary>
public static class Emojis
{
    private static readonly Lazy<IReadOnlyList<Emoji>> emojis = new(() =>
    {
        var cps = EmojiShortcodesDatabase.PerLanguage
            .SelectMany(static x => x.Value) // All languages
            .SelectMany(static x => x.Value) // All databases
            .Select(static x => x.Key) // Get codepoints
            .Select(static x => new Emoji(x))
            .Distinct()
            .OrderBy(static x => x)
            .ToArray()
            .AsReadOnly();
        return cps;
    });

    /// <summary>
    /// Gets the list of all emojis.
    /// </summary>
    public static IReadOnlyList<Emoji> All => emojis.Value;
}
