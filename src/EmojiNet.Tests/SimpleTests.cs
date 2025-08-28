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
                    perCodepointEntry.Key.Should().NotBeEmpty();
                    perCodepointEntry.Value.Should().NotBeEmpty();
                }
            }
        }
    }

    [Fact]
    public static void HasAllLanguages()
        => EmojiShortcodes.LanguageNames.Should()
        .BeEquivalentTo(
            "bn",
            "da",
            "de",
            "en",
            "en-gb",
            "es",
            "es-mx",
            "et",
            "fi",
            "fr",
            "hi",
            "hu",
            "it",
            "ja",
            "ko",
            "lt",
            "ms",
            "nb",
            "nl",
            "pl",
            "pt",
            "ru",
            "sv",
            "th",
            "uk",
            "vi",
            "zh",
            "zh-hant");

    [Fact]
    public static void HasAllDatabases()
        => EmojiShortcodes.DatabaseNames.Should()
        .BeEquivalentTo(
            "cldr",
            "cldr-native",
            "emojibase",
            "emojibase-legacy",
            "emojibase-native",
            "github",
            "iamcal",
            "joypixels");
}
