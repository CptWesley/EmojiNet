namespace EmojiNet;

public static partial class EmojiShortcodes
{
    private static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
#if NET8_0_OR_GREATER
        return System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(dictionary);
#else
        return new System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary);
#endif
    }
}
