using System.Net.Http;

namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Reponsible for downloading files.
/// </summary>
public static class FileDownloader
{
    private static readonly HttpClient client = new HttpClient();

    static FileDownloader()
    {
        client.DefaultRequestHeaders.Add("User-Agent", typeof(GitRepositoryDownloader).Assembly.GetName().Name);
    }

    /// <summary>
    /// Downloads the content at the given <paramref name="url"/> as a <see cref="string"/>.
    /// </summary>
    /// <param name="url">The URL to download from.</param>
    /// <returns>The content at the given <paramref name="url"/> read as a <see cref="string"/>.</returns>
    public static async Task<string> DownloadAsStringAsync(string url)
    {
        using var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return json;
    }
}
