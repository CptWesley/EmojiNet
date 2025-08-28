namespace EmojiNet;

/// <summary>
/// Represents an emoji.
/// </summary>
public sealed class Emoji : IEquatable<Emoji>, IComparable<Emoji>
{
    private static readonly int typeHashcode = 95679037;

    private readonly string str;
    private readonly int hashcode;
    private readonly Lazy<EmojiShortcodes> shortcodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="Emoji"/> class.
    /// </summary>
    /// <param name="codePoints">The codepoints.</param>
    internal Emoji(
        IReadOnlyList<int> codePoints)
    {
        CodePoints = codePoints;
        shortcodes = new(() => new EmojiShortcodes(this));

        var sb = new StringBuilder();
        foreach (var cp in codePoints)
        {
            var c = char.ConvertFromUtf32(cp);
            sb.Append(c);
        }

        str = sb.ToString();

        hashcode = 7706329;
        hashcode = hashcode * 7362133 ^ codePoints.Count.GetHashCode();

        for (var i = 0; i < codePoints.Count; i++)
        {
            hashcode = hashcode * 7362133 ^ (i, codePoints[i]).GetHashCode();
        }

        hashcode += typeHashcode;
    }

    /// <summary>
    /// The codepoints of this emoji.
    /// </summary>
    public IReadOnlyList<int> CodePoints { get; }

    /// <summary>
    /// The shortcodes of this emoji.
    /// </summary>
    public EmojiShortcodes Shortcodes => shortcodes.Value;

    /// <inheritdoc />
    public override string ToString()
        => str;

    /// <inheritdoc />
    public override int GetHashCode()
        => hashcode;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Emoji other && Equals(other);

    /// <inheritdoc />
    public bool Equals(Emoji? other)
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

        return CodePoints.SequenceEqual(other.CodePoints);
    }

    /// <inheritdoc />
    public int CompareTo(Emoji? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        var a = this;
        var b = other;

        var minCount = Math.Min(a.CodePoints.Count, b.CodePoints.Count);

        for (var i = 0; i < minCount; i++)
        {
            var av = a.CodePoints[i];
            var bv = b.CodePoints[i];

            var compared = av.CompareTo(bv);
            if (compared != 0)
            {
                return compared;
            }
        }

        return a.CodePoints.Count.CompareTo(b.CodePoints.Count);
    }

    /// <summary>
    /// Checks if <paramref name="left"/> and <paramref name="right"/> are equal.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if both values are equal;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator ==(Emoji? left, Emoji? right)
    {
        if (left is null)
        {
            return right is null;
        }
        else
        {
            return left.Equals(right);
        }
    }

    /// <summary>
    /// Checks if <paramref name="left"/> and <paramref name="right"/> are unequal.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if both values are unequal;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator !=(Emoji? left, Emoji? right)
        => !(left == right);

    /// <summary>
    /// Checks if <paramref name="left"/> is less than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator <(Emoji? left, Emoji? right)
    {
        if (left is null)
        {
            return right is not null;
        }
        else
        {
            return left.CompareTo(right) < 0;
        }
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is less than or equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator <=(Emoji? left, Emoji? right)
    {
        if (left is null)
        {
            return true;
        }
        else
        {
            return left.CompareTo(right) <= 0;
        }
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is greater than <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator >(Emoji? left, Emoji? right)
    {
        if (left is null)
        {
            return false;
        }
        else
        {
            return left.CompareTo(right) > 0;
        }
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is greater than or equal <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> is greater than or equal <paramref name="right"/>;
    /// <see langword="false"/> otherwise.
    /// </returns>
    public static bool operator >=(Emoji left, Emoji right)
    {
        if (left is null)
        {
            return right is null;
        }
        else
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
