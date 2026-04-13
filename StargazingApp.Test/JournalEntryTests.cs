using StargazingApp.Tests.Stubs;

namespace StargazingApp.Tests;

/// <summary>
/// Tests for JournalEntry: MatchesSearch, RelativeDate, RatingDisplay, FormattedDate.
/// No database or MAUI dependencies — pure logic only.
/// </summary>
public class JournalEntryTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>Creates a fully-populated entry for use in tests.</summary>
    private static JournalEntry MakeEntry(
        string title      = "Orion Observation",
        string body       = "Spotted the belt clearly",
        string conditions = "Clear skies",
        int    rating     = 3,
        DateTime? createdAt = null)
    {
        return new JournalEntry
        {
            Title      = title,
            Body       = body,
            Conditions = conditions,
            Rating     = rating,
            CreatedAt  = createdAt ?? DateTime.Now
        };
    }

    // =========================================================================
    // MatchesSearch
    // =========================================================================

    [Fact]
    public void MatchesSearch_EmptyTerm_ReturnsTrue()
    {
        var entry = MakeEntry();
        Assert.True(entry.MatchesSearch(string.Empty));
    }

    [Fact]
    public void MatchesSearch_WhitespaceTerm_ReturnsTrue()
    {
        var entry = MakeEntry();
        Assert.True(entry.MatchesSearch("   "));
    }

    [Fact]
    public void MatchesSearch_NullTerm_ReturnsTrue()
    {
        var entry = MakeEntry();
        Assert.True(entry.MatchesSearch(null!));
    }

    [Fact]
    public void MatchesSearch_TermMatchesTitle_ReturnsTrue()
    {
        var entry = MakeEntry(title: "Jupiter at opposition");
        Assert.True(entry.MatchesSearch("Jupiter"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesTitleCaseInsensitive_ReturnsTrue()
    {
        var entry = MakeEntry(title: "Jupiter at opposition");
        Assert.True(entry.MatchesSearch("JUPITER"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesPartialTitle_ReturnsTrue()
    {
        var entry = MakeEntry(title: "Jupiter at opposition");
        Assert.True(entry.MatchesSearch("oppos"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesBody_ReturnsTrue()
    {
        var entry = MakeEntry(body: "Clearly saw the Great Red Spot");
        Assert.True(entry.MatchesSearch("Red Spot"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesConditions_ReturnsTrue()
    {
        var entry = MakeEntry(conditions: "No moon, Bortle 4");
        Assert.True(entry.MatchesSearch("bortle"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesNowhere_ReturnsFalse()
    {
        var entry = MakeEntry(title: "Orion", body: "Nice view", conditions: "Clear");
        Assert.False(entry.MatchesSearch("andromeda"));
    }

    [Fact]
    public void MatchesSearch_TermMatchesBodyButNotTitleOrConditions_ReturnsTrue()
    {
        var entry = MakeEntry(title: "Night out", body: "Saw Pleiades", conditions: "Windy");
        Assert.True(entry.MatchesSearch("pleiades"));
    }

    // =========================================================================
    // RelativeDate
    // =========================================================================

    [Fact]
    public void RelativeDate_CreatedToday_ReturnsToday()
    {
        var entry = MakeEntry(createdAt: DateTime.Now);
        Assert.Equal("Today", entry.RelativeDate);
    }

    [Fact]
    public void RelativeDate_CreatedYesterday_ReturnsYesterday()
    {
        var entry = MakeEntry(createdAt: DateTime.Now.AddDays(-1));
        Assert.Equal("Yesterday", entry.RelativeDate);
    }

    [Fact]
    public void RelativeDate_CreatedThreeDaysAgo_ReturnsDaysAgoString()
    {
        var entry = MakeEntry(createdAt: DateTime.Now.AddDays(-3));
        Assert.Equal("3 days ago", entry.RelativeDate);
    }

    [Fact]
    public void RelativeDate_CreatedSevenDaysAgo_ReturnsDaysAgoString()
    {
        var entry = MakeEntry(createdAt: DateTime.Now.AddDays(-7));
        Assert.Equal("7 days ago", entry.RelativeDate);
    }

    [Fact]
    public void RelativeDate_CreatedEightDaysAgo_ReturnsFormattedDate()
    {
        var date  = DateTime.Now.AddDays(-8);
        var entry = MakeEntry(createdAt: date);
        Assert.Equal(date.ToString("MMM dd, yyyy"), entry.RelativeDate);
    }

    [Fact]
    public void RelativeDate_CreatedOneYearAgo_ReturnsFormattedDate()
    {
        var date  = DateTime.Now.AddYears(-1);
        var entry = MakeEntry(createdAt: date);
        Assert.Equal(date.ToString("MMM dd, yyyy"), entry.RelativeDate);
    }

    // =========================================================================
    // RatingDisplay
    // =========================================================================

    [Theory]
    [InlineData(1, "⭐")]
    [InlineData(2, "⭐⭐")]
    [InlineData(3, "⭐⭐⭐")]
    [InlineData(4, "⭐⭐⭐⭐")]
    [InlineData(5, "⭐⭐⭐⭐⭐")]
    public void RatingDisplay_ValidRatings_ReturnsCorrectStars(int rating, string expected)
    {
        var entry = MakeEntry(rating: rating);
        Assert.Equal(expected, entry.RatingDisplay);
    }

    [Fact]
    public void RatingDisplay_RatingBelowOne_ClampsToOneStar()
    {
        // Math.Clamp(0, 1, 5) => 1
        var entry = MakeEntry(rating: 0);
        Assert.Equal("⭐", entry.RatingDisplay);
    }

    [Fact]
    public void RatingDisplay_RatingAboveFive_ClampsToFiveStars()
    {
        // Math.Clamp(99, 1, 5) => 5
        var entry = MakeEntry(rating: 99);
        Assert.Equal("⭐⭐⭐⭐⭐", entry.RatingDisplay);
    }

    [Fact]
    public void RatingDisplay_NegativeRating_ClampsToOneStar()
    {
        var entry = MakeEntry(rating: -5);
        Assert.Equal("⭐", entry.RatingDisplay);
    }

    // =========================================================================
    // FormattedDate
    // =========================================================================

    [Fact]
    public void FormattedDate_ReturnsCorrectFormat()
    {
        var date  = new DateTime(2025, 3, 15, 21, 45, 0);
        var entry = MakeEntry(createdAt: date);
        Assert.Equal("Mar 15, 2025  •  21:45", entry.FormattedDate);
    }

    [Fact]
    public void FormattedDate_MidnightEntry_ShowsZeroHour()
    {
        var date  = new DateTime(2025, 1, 1, 0, 0, 0);
        var entry = MakeEntry(createdAt: date);
        Assert.Equal("Jan 01, 2025  •  00:00", entry.FormattedDate);
    }

    [Fact]
    public void FormattedDate_SingleDigitDay_PadsWithZero()
    {
        var date  = new DateTime(2025, 6, 5, 20, 30, 0);
        var entry = MakeEntry(createdAt: date);
        Assert.Equal("Jun 05, 2025  •  20:30", entry.FormattedDate);
    }

    // =========================================================================
    // Inheritance & Interface
    // =========================================================================

    [Fact]
    public void JournalEntry_ImplementsISearchable()
    {
        // Verifies the interface contract is fulfilled (marks the Inheritance requirement)
        ISearchable searchable = new JournalEntry { Title = "test" };
        Assert.True(searchable.MatchesSearch("test"));
    }

    [Fact]
    public void JournalEntry_InheritsFromEntryBase()
    {
        var entry = new JournalEntry();
        Assert.IsAssignableFrom<EntryBase>(entry);
    }

    [Fact]
    public void EntryBase_DefaultCreatedAt_IsSetOnConstruction()
    {
        var before = DateTime.Now.AddSeconds(-1);
        var entry  = new JournalEntry();
        var after  = DateTime.Now.AddSeconds(1);

        Assert.InRange(entry.CreatedAt, before, after);
    }
}
