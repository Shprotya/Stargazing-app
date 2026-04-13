// Minimal copies of production types used in tests.
// These mirror StargazingApp exactly — keep them in sync if you change the originals.

namespace StargazingApp.Tests.Stubs;

// ── EntryBase ────────────────────────────────────────────────────────────────

public abstract class EntryBase
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

// ── ISearchable ──────────────────────────────────────────────────────────────

public interface ISearchable
{
    bool MatchesSearch(string term);
}

// ── JournalEntry ─────────────────────────────────────────────────────────────

public class JournalEntry : EntryBase, ISearchable
{
    public string Title      { get; set; } = string.Empty;
    public string Body       { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int    Rating     { get; set; } = 3;

    public bool MatchesSearch(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return true;
        var t = term.ToLower();
        return Title.ToLower().Contains(t)
            || Body.ToLower().Contains(t)
            || Conditions.ToLower().Contains(t);
    }

    public string FormattedDate => CreatedAt.ToString("MMM dd, yyyy  •  HH:mm");

    public string RelativeDate
    {
        get
        {
            var days = (DateTime.Now.Date - CreatedAt.Date).Days;
            return days switch
            {
                0       => "Today",
                1       => "Yesterday",
                <= 7    => $"{days} days ago",
                _       => CreatedAt.ToString("MMM dd, yyyy")
            };
        }
    }

    public string RatingDisplay => new string('⭐', Math.Clamp(Rating, 1, 5));
}

// ── SevenTimerEntry / SevenTimerService ──────────────────────────────────────

public class SevenTimerEntry
{
    public int Timepoint    { get; set; }
    public int CloudCover   { get; set; }
    public int Seeing       { get; set; }
    public int Transparency { get; set; }
}

public class SevenTimerService
{
    public string GetVisibilityRating(SevenTimerEntry entry)
    {
        string rating;
        bool atmospherePoor = entry.Seeing >= 5 || entry.Transparency >= 5;

        if (entry.CloudCover >= 7)
            rating = "🔴 Too cloudy...";
        else if (entry.CloudCover >= 5 || (entry.CloudCover >= 3 && atmospherePoor))
            rating = "🟠 Poor";
        else if (entry.CloudCover >= 3 || atmospherePoor)
            rating = "🟡 Fair";
        else if (entry.CloudCover <= 2 && entry.Seeing <= 3 && entry.Transparency <= 3)
            rating = "🟢 Excellent";
        else
            rating = "🟡 Fair";

        var cloudText = entry.CloudCover switch
        {
            1 => "0–6%",   2 => "6–19%",  3 => "19–31%",
            4 => "31–44%", 5 => "44–56%", 6 => "56–69%",
            7 => "69–81%", 8 => "81–94%", 9 => "94–100%",
            _ => "Unknown"
        };

        var seeingText = entry.Seeing switch
        {
            1 => "Excellent", 2 => "Good",      3 => "Fair",
            4 => "Moderate",  5 => "Poor",       6 => "Very Poor",
            7 => "Bad",       8 => "Terrible",   _ => "Unknown"
        };

        var transparencyText = entry.Transparency switch
        {
            1 => "Excellent", 2 => "Good",      3 => "Fair",
            4 => "Moderate",  5 => "Poor",       6 => "Very Poor",
            7 => "Bad",       8 => "Terrible",   _ => "Unknown"
        };

        return $"{rating}\n☁️ Cloud cover: {cloudText}\n- Above Cloud Data:\n👁️ Seeing: {seeingText}\n🔭 Transparency: {transparencyText}";
    }
}
