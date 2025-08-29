using EmojiNet.Internal;

namespace EmojiNet.Tests;

public static class SimpleTests
{
    private const int KnownEmojiCount = 3816;

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
    public static void FoundCorrectNumberOfEmojisInList()
        => Emojis.All.Should().HaveCount(KnownEmojiCount);

    [Fact]
    public static void FoundCorrectNumberOfEmojisInLookup()
        => Emojis.Lookup.Should().HaveCount(KnownEmojiCount);

#pragma warning disable QW0004, S3257
    [Theory]
    [InlineData(new string[] { }, new string[] { }, "smile", null)]
    [InlineData(new string[] { "en" }, new string[] { }, "smile", null)]
    [InlineData(new string[] { "en" }, new string[] { "cldr" }, "smile", null)]
    [InlineData(new string[] { "en" }, new string[] { "cldr", "github" }, "smile", "😄")]
    [InlineData(new string[] { "da" }, new string[] { "cldr", "github" }, "smile", null)]
    [InlineData(new string[] { "en" }, new string[] { "emojibase-legacy" }, "smile", "😃")]
    [InlineData(new string[] { "en" }, new string[] { "emojibase" }, "smile", "😄")]
    [InlineData(new string[] { "en" }, new string[] { "emojibase", "emojibase-legacy" }, "smile", "😄")]
    [InlineData(new string[] { "en" }, new string[] { "emojibase-legacy", "emojibase" }, "smile", "😃")]
    public static void SimpleResolver(string[] languages, string[] databases, string shortCode, string? expectedEmoji)
    {
        var resolver = Emojis.GetResolver(languages, databases);

        if (expectedEmoji is not null)
        {
            resolver.TryGetValue(shortCode, out var foundEmoji).Should().BeTrue();
            foundEmoji.Should().NotBeNull();
            foundEmoji.ToString().Should().Be(expectedEmoji);
        }
        else
        {
            resolver.TryGetValue(shortCode, out var foundEmoji).Should().BeFalse();
            foundEmoji.Should().BeNull();
        }
    }
#pragma warning restore QW0004, S3257
}
