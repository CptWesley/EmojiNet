namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Used to download the emojibase databases.
/// </summary>
public static partial class Program
{
    private const string Organization = "milesj";
    private const string Repository = "emojibase";
    private const string Branch = "master";

    [GeneratedRegex(@"packages\/data\/(?<lang>\w+)\/shortcodes\/(?<source>\w+).raw.json", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex FileNameRegex { get; }

    /// <summary>
    /// Entry point of the application.
    /// </summary>
    /// <param name="args">The arguments of the application.</param>
    /// <returns>The task executing the application.</returns>
    public static async Task Main(params string[] args)
    {
        var tree = await GitRepositoryDownloader.GetTreeAsync(Organization, Repository, Branch);

        foreach (var file in tree.Tree)
        {
            if (file.Path is not { Length: > 0 } path
             || FileNameRegex.Match(path) is not { Success: true } match)
            {
                continue;
            }

            var lang = match.Groups["lang"].Value;
            var source = match.Groups["source"].Value;

            var url = $"https://raw.githubusercontent.com/{Organization}/{Repository}/{Branch}/packages/data/{lang}/shortcodes/{source}.raw.json";

            var content = await FileDownloader.DownloadAsStringAsync(url);
        }

        Console.WriteLine("Hello, World!");
    }
}
