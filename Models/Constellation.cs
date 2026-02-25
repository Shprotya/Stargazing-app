using SQLite;

namespace StargazingApp.Models;

public class Constellation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed] // Makes searching by name much faster
    public string Name { get; set; }

    public string Abbreviation { get; set; }
    public string Description { get; set; }
    public string BestVisibleMonth { get; set; }

    [Indexed] // Makes filtering by hemisphere faster
    public string Hemisphere { get; set; }

    public string VisibleLatitude { get; set; }
    public string BrightestStar { get; set; }
    public int NumberOfStars { get; set; }
    public int Area { get; set; }
    public string ImageUrl { get; set; }

    public bool IsFavorite { get; set; }
}