using SQLite;

namespace StargazingApp.Models;

public class JournalEntry 
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Conditions { get; set; } = string.Empty;
    public int Rating { get; set; } = 3;

    
}
