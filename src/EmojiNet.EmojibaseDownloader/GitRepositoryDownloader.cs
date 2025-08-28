using System.Text.Json;

namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Downloads git repositories.
/// </summary>
public static class GitRepositoryDownloader
{
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerOptions.Web);

    static GitRepositoryDownloader()
    {
        serializerOptions.PropertyNameCaseInsensitive = true;
    }

    /// <summary>
    /// Gets the repository tree.
    /// </summary>
    /// <param name="organization">The organization that owns the repository.</param>
    /// <param name="repository">The repository to download.</param>
    /// <param name="branch">The branch to retrieve from.</param>
    /// <returns>The downloaded repository tree.</returns>
    public static async Task<GitRepositoryTree> GetTreeAsync(string organization, string repository, string branch)
    {
        var url = $"https://api.github.com/repos/{organization}/{repository}/git/trees/{branch}?recursive=1";
        var json = await FileDownloader.DownloadAsStringAsync(url);
        var result = JsonSerializer.Deserialize<GitRepositoryTree>(
            json: json,
            serializerOptions);

        if (result is null)
        {
            throw new InvalidOperationException();
        }

        return result;
    }
}
