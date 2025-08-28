namespace EmojiNet.Internal;

/// <summary>
/// Provides a read-only shell over an array.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
internal sealed class ReadOnlyList<T> : IReadOnlyList<T>
{
    private readonly T[] array;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="array">The array to wrap.</param>
    public ReadOnlyList(T[] array)
    {
        this.array = array;
    }

    /// <inheritdoc />
    public T this[int index] => array[index];

    /// <inheritdoc />
    public int Count => array.Length;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => ((IReadOnlyList<T>)array).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
