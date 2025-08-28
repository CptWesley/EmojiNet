using EmojiNet.Internal;

namespace EmojiNet.Tests;

public static class SimpleTests
{
    [Fact]
    public static void HasAnyData()
    {
        EmojiShortcodesDatabase.PerLanguage.Should().NotBeEmpty();

        foreach (var perLanguageEntry in EmojiShortcodesDatabase.PerLanguage)
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
        => EmojiShortcodesDatabase.LanguageNames.Should()
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
        => EmojiShortcodesDatabase.DatabaseNames.Should()
        .BeEquivalentTo(
            "cldr",
            "cldr-native",
            "emojibase",
            "emojibase-legacy",
            "emojibase-native",
            "github",
            "iamcal",
            "joypixels");

    [Fact]
    public static void FoundCorrectNumberOfEmojis()
        => Emojis.All.Should().HaveCount(3816);
}
