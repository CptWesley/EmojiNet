namespace EmojiNet.SourceGenerator.Tests;

public sealed class EmojiDatabaseGeneratorTests
{
    [Fact]
    public void Generates()
    {
        GeneratorSnapshot.Verify<EmojiDatabaseGenerator>(string.Empty);
    }

    [Fact]
    public void Incremental()
    {
        GeneratorIncrementalCache.Verify<EmojiDatabaseGenerator>(string.Empty);
    }
}
