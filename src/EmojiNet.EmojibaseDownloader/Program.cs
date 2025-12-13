using System.IO.Compression;
using System.Text.Json;

namespace EmojiNet.EmojibaseDownloader;

/// <summary>
/// Used to download the emojibase databases.
/// </summary>
public static partial class Program
{
    private const string Organization = "milesj";
    private const string Repository = "emojibase";
    private const string Branch = "master";

    [GeneratedRegex(@"packages\/data\/(?<lang>[\w\-]+)\/shortcodes\/(?<source>[\w\-]+).raw.json", RegexOptions.Compiled | RegexOptions.ExplicitCapture)]
    private static partial Regex FileNameRegex { get; }

    /// <summary>
    /// Entry point of the application.
    /// </summary>
    /// <param name="args">The arguments of the application.</param>
    /// <returns>The task executing the application.</returns>
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
    public static async Task Main(params string[] args)
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
    {
        var outputDir = GetOutputPath();
        var gzipFilePath = Path.Combine(outputDir, "gzip.bin");
        var brotliFilePath = Path.Combine(outputDir, "brotli.bin");

        var tree = await GitRepositoryDownloader.GetTreeAsync(Organization, Repository, Branch);

        using var gzipFileStream = File.OpenWrite(gzipFilePath);
        using var gzipCompressionStream = new GZipStream(gzipFileStream, CompressionLevel.SmallestSize);
        using var gzipWriter = new StreamWriter(gzipCompressionStream);

        using var brotliFileStream = File.OpenWrite(brotliFilePath);
        using var brotliCompressionStream = new BrotliStream(brotliFileStream, CompressionLevel.SmallestSize);
        using var brotliWriter = new StreamWriter(brotliCompressionStream);
        Console.WriteLine($"Writing to file '{gzipFilePath}'...");
        Console.WriteLine($"Writing to file '{brotliFilePath}'...");

        Console.WriteLine("Starting to download all data...");

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

            Console.WriteLine($"Downloading '{url}'...");
            var content = await FileDownloader.DownloadAsStringAsync(url);
            Console.WriteLine($"Finished downloading '{url}'.");

            await gzipWriter.WriteLineAsync($"=\t{lang}\t{source}");
            await brotliWriter.WriteLineAsync($"=\t{lang}\t{source}");
            using var doc = JsonDocument.Parse(content).RootElement.EnumerateObject();

            foreach (var prop in doc)
            {
                if (prop.Name?.Trim() is not { Length: > 0 } name)
                {
                    continue;
                }

                var value = prop.Value;
                var valueList = new List<string>();

                if (value.ValueKind == JsonValueKind.String && value.GetString()?.Trim() is { Length: > 0 } str)
                {
                    valueList.Add(str);
                }
                else if (value.ValueKind == JsonValueKind.Array)
                {
                    using var array = value.EnumerateArray();

                    foreach (var item in array)
                    {
                        if (item.GetString()?.Trim() is { Length: > 0 } entry)
                        {
                            valueList.Add(entry);
                        }
                    }
                }

                await gzipWriter.WriteLineAsync($"{name}\t{string.Join('\t', valueList)}");
                await brotliWriter.WriteLineAsync($"{name}\t{string.Join('\t', valueList)}");
            }
        }

        Console.WriteLine("Finished downloading all data.");

        await gzipWriter.FlushAsync();
        await gzipCompressionStream.FlushAsync();
        await gzipFileStream.FlushAsync();

        Console.WriteLine($"Finished writing to file '{gzipFilePath}'.");

        await brotliWriter.FlushAsync();
        await brotliCompressionStream.FlushAsync();
        await brotliFileStream.FlushAsync();

        Console.WriteLine($"Finished writing to file '{brotliFilePath}'.");
    }

    private static string GetOutputPath([CallerFilePath] string? path = null)
    {
        var curDir = path is null
            ? Directory.GetCurrentDirectory()
            : Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();

        var outputDir = Path.GetFullPath(Path.Combine(curDir, "../EmojiNet/Resources/Shortcodes"));

        if (Directory.Exists(outputDir))
        {
            Console.WriteLine($"Deleting output directory '{outputDir}'...");
            Directory.Delete(outputDir, true);
            Console.WriteLine($"Deleted output directory '{outputDir}'.");
            Console.WriteLine($"Recreating output directory '{outputDir}'...");
            Directory.CreateDirectory(outputDir);
            Console.WriteLine($"Recreated output directory '{outputDir}'.");
        }
        else
        {
            Console.WriteLine($"Creating output directory '{outputDir}'...");
            Directory.CreateDirectory(outputDir);
            Console.WriteLine($"Created output directory '{outputDir}'.");
        }

        return outputDir;
    }
}
