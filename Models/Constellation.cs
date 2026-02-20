using SQLite;

namespace StargazingApp.Models;

public class Constellation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed] // Makes searching by name much faster
    public string Name { get; set; }

    public string Description { get; set; }
    public string BestVisibleMonth { get; set; }
    public bool IsFavorite { get; set; }
}