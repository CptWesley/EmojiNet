namespace EmojiNet.Tests;

public static class SimpleTests
{
    [Fact]
    public static void HasAnyData()
    {
        EmojiShortcodes.PerLanguage.Should().NotBeEmpty();

        foreach (var perLanguageEntry in EmojiShortcodes.PerLanguage)
        {
            perLanguageEntry.Key.Should().NotBeNullOrWhiteSpace();
            perLanguageEntry.Value.Should().NotBeEmpty();

            foreach (var perSourceEntry in perLanguageEntry.Value)
            {
                perSourceEntry.Key.Should().NotBeNullOrWhiteSpace();
                perSourceEntry.Value.Should().NotBeEmpty();

                foreach (var perCodepointEntry in perSourceEntry.Value)
                {
                    perCodepointEntry.Key.Should().NotBeNullOrWhiteSpace();
                    perCodepointEntry.Value.Should().NotBeEmpty();
                }
            }
        }
    }
}
