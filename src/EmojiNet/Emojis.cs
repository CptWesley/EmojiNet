#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace EmojiNet;

/// <summary>
/// Provides a database of all known emojis.
/// </summary>
public static class Emojis
{
    private static readonly Lazy<IReadOnlyList<Emoji>> list = new(() =>
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

    private static readonly Lazy<IReadOnlyDictionary<string, Emoji>> lookup = new(() =>
    {
        return list.Value.ToDictionary(static x => x.ToString(), static x => x).AsReadOnly();
    });

#if NET8_0_OR_GREATER
    private static readonly Lazy<FrozenSet<string>> languageSet = new(() => LanguageNames.ToFrozenSet());
    private static readonly Lazy<FrozenSet<string>> databaseSet = new(() => DatabaseNames.ToFrozenSet());
#else
    private static readonly Lazy<HashSet<string>> languageSet = new(() => new HashSet<string>(LanguageNames));
    private static readonly Lazy<HashSet<string>> databaseSet = new(() => new HashSet<string>(DatabaseNames));
#endif

    private static readonly ConcurrentDictionary<ResolverCacheKey, EmojiShortcodeResolver> resolverCache = new();

    /// <summary>
    /// Gets the list of all emojis.
    /// </summary>
    public static IReadOnlyList<Emoji> All => list.Value;

    /// <summary>
    /// Gets a lookup table for all emojis.
    /// </summary>
    public static IReadOnlyDictionary<string, Emoji> Lookup => lookup.Value;

    /// <inheritdoc cref="EmojiShortcodesDatabase.LanguageNames" />
    public static IReadOnlyList<string> LanguageNames => EmojiShortcodesDatabase.LanguageNames;

    /// <inheritdoc cref="EmojiShortcodesDatabase.DatabaseNames" />
    public static IReadOnlyList<string> DatabaseNames => EmojiShortcodesDatabase.DatabaseNames;

    /// <summary>
    /// Gets the resolved for the given <paramref name="languages"/> and <paramref name="databases"/>.
    /// </summary>
    /// <param name="languages">The languages to create the resolver for.</param>
    /// <param name="databases">The databases to create the resolver for.</param>
    /// <returns>The resolver that resolver emoji shortcodes based on the given <paramref name="languages"/> and <paramref name="databases"/>.</returns>
    public static EmojiShortcodeResolver GetResolver(IEnumerable<string> languages, IEnumerable<string> databases)
    {
        var l = languages
            .Distinct()
            .Where(static x => languageSet.Value.Contains(x))
            .ToArray();
        var d = databases
            .Distinct()
            .Where(static x => databaseSet.Value.Contains(x))
            .ToArray();
        var key = new ResolverCacheKey(l, d);

        var result = resolverCache.GetOrAdd(key, static key =>
        {
            return new EmojiShortcodeResolver(key.Languages, key.Databases);
        });

        return result;
    }

    /// <summary>
    /// Gets the resolved for the given <paramref name="language"/> and <paramref name="databases"/>.
    /// </summary>
    /// <param name="language">The language to create the resolver for.</param>
    /// <param name="databases">The databases to create the resolver for.</param>
    /// <returns>The resolver that resolver emoji shortcodes based on the given <paramref name="language"/> and <paramref name="databases"/>.</returns>
    public static EmojiShortcodeResolver GetResolver(string language, IEnumerable<string> databases)
        => GetResolver(languages: [language], databases: databases);

    /// <summary>
    /// Gets the resolved for the given <paramref name="languages"/> and <paramref name="database"/>.
    /// </summary>
    /// <param name="languages">The languages to create the resolver for.</param>
    /// <param name="database">The database to create the resolver for.</param>
    /// <returns>The resolver that resolver emoji shortcodes based on the given <paramref name="languages"/> and <paramref name="database"/>.</returns>
    public static EmojiShortcodeResolver GetResolver(IEnumerable<string> languages, string database)
        => GetResolver(languages: languages, databases: [database]);

    /// <summary>
    /// Gets the resolved for the given <paramref name="language"/> and <paramref name="database"/>.
    /// </summary>
    /// <param name="language">The language to create the resolver for.</param>
    /// <param name="database">The database to create the resolver for.</param>
    /// <returns>The resolver that resolver emoji shortcodes based on the given <paramref name="language"/> and <paramref name="database"/>.</returns>
    public static EmojiShortcodeResolver GetResolver(string language, string database)
        => GetResolver(languages: [language], databases: [database]);

    private sealed class ResolverCacheKey : IEquatable<ResolverCacheKey>
    {
        private static readonly int typeHashcode = 585214703;

        private readonly int hashcode;

        public ResolverCacheKey(string[] languages, string[] databases)
        {
            Languages = languages;
            Databases = databases;

            hashcode = 7706329;
            hashcode = hashcode * 7362133 ^ languages.Length.GetHashCode();

            for (var i = 0; i < languages.Length; i++)
            {
                hashcode = hashcode * 7362133 ^ (i, languages[i]).GetHashCode();
            }

            hashcode = hashcode * 7362133 ^ databases.Length.GetHashCode();

            for (var i = 0; i < databases.Length; i++)
            {
                hashcode = hashcode * 7362133 ^ (i, databases[i]).GetHashCode();
            }

            hashcode += typeHashcode;
        }

        public string[] Languages { get; }

        public string[] Databases { get; }

        public override int GetHashCode()
            => hashcode;

        public override bool Equals(object? obj)
            => obj is ResolverCacheKey other
            && Equals(other);

        public bool Equals(ResolverCacheKey? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (hashcode != other.hashcode)
            {
                return false;
            }

            return Languages.SequenceEqual(other.Languages)
                && Databases.SequenceEqual(other.Databases);
        }
    }
}
