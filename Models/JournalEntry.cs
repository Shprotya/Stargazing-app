using SQLite;

namespace StargazingApp.Models;

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

public class JournalEntry 
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int Rating { get; set; } = 3;

    
}
