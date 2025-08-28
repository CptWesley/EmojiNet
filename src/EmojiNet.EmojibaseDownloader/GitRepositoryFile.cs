namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Represents a git repository file object.
/// </summary>
public sealed record GitRepositoryFile
{
    /// <summary>
    /// The path of the file.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// The file mode.
    /// </summary>
    public string? Mode { get; init; }

    /// <summary>
    /// the file type.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// The SHA hash of the file.
    /// </summary>
    public string? Sha { get; init; }

    /// <summary>
    /// The file size.
    /// </summary>
    public int? Size { get; init; }

    /// <summary>
    /// The API URL of the file.
    /// </summary>
    public string? Url { get; init; }
}
