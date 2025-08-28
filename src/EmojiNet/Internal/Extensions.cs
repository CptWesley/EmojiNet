namespace EmojiNet.Internal;

/// <summary>
/// Provides extension methods.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Converts the given <paramref name="dictionary"/> to a read-only variant.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="dictionary">The dictionary to convert.</param>
    /// <returns>The read-only variant of the dictionary.</returns>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
        {
#if NET8_0_OR_GREATER
            return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dictionary);
#else
            return new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary);
#endif
        }

    /// <inheritdoc cref="AsReadOnly{TKey, TValue}(Dictionary{TKey, TValue})" />
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TSource, TKey, TValue>(
        this IEnumerable<TSource> dictionary,
        Func<TSource, TKey> keySelector,
        Func<TSource, TValue> valueSelector)
        where TKey : notnull
    {
#if NET8_0_OR_GREATER
            return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dictionary, keySelector, valueSelector);
#else
        return new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary.ToDictionary(keySelector, valueSelector));
#endif
    }

    /// <summary>
    /// Converts the given <paramref name="array"/> to a read-only variant.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="array">The array to convert.</param>
    /// <returns>The read-only variant of the array.</returns>
    public static IReadOnlyList<T> AsReadOnly<T>(this T[] array)
        => new ReadOnlyList<T>(array);
}
