using SQLite;

namespace StargazingApp.Models;

/// <summary>
/// Implemented by any model that supports full-text search.
/// </summary>
public interface ISearchable
{
    bool MatchesSearch(string term);
}

/// <summary>
/// Base class for all user-created log entries.
/// Provides common audit fields shared across entry types.
/// </summary>
public abstract class EntryBase
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

/// <summary>
/// Represents a personal astronomy journal entry stored in SQLite.
/// </summary>
public class JournalEntry : EntryBase, ISearchable
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int Rating { get; set; } = 3;

    // ISearchable
    public bool MatchesSearch(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return true;
        var t = term.ToLower();
        return Title.ToLower().Contains(t)
            || Body.ToLower().Contains(t)
            || Conditions.ToLower().Contains(t);
    }

    /// <summary>Formatted display string for the creation date.</summary>
    [Ignore]
    public string FormattedDate => CreatedAt.ToString("MMM dd, yyyy  •  HH:mm");

    /// <summary>Returns a short relative description such as "Today" or "3 days ago".</summary>
    [Ignore]
    public string RelativeDate
    {
        get
        {
            var days = (DateTime.Now.Date - CreatedAt.Date).Days;
            return days switch
            {
                0 => "Today",
                1 => "Yesterday",
                <= 7 => $"{days} days ago",
                _ => CreatedAt.ToString("MMM dd, yyyy")
            };
        }
    }

    /// <summary>Star emoji display for rating.</summary>
    [Ignore]
    public string RatingDisplay => new string('⭐', Math.Clamp(Rating, 1, 5));
}
