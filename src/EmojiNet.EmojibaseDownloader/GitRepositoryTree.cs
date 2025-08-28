using System.Collections.Immutable;

namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Represents a git repository tree object.
/// </summary>
public sealed record GitRepositoryTree
{
    /// <summary>
    /// The API URL for this repository.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// The SHA hash of the current version.
    /// </summary>
    public string? Sha { get; init; }

    /// <summary>
    /// The files stored in this repository.
    /// </summary>
    public ImmutableArray<GitRepositoryFile> Tree { get; init; } = [];
}
