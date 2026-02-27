using SQLite;
using StargazingApp.Models;
using Newtonsoft.Json;

public class DatabaseService
{
    
    private SQLiteAsyncConnection _database;
    // This runs once and sets everything up
    private async Task Init()
    {
        if (_database is not null) return; // if already set up, do nothing

        // Find where to save the file on the device
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Stargazing.db3");

        // Open (or create) the database file
        _database = new SQLiteAsyncConnection(dbPath);

        // Create the Constellations table if it doesn't exist
        await _database.CreateTableAsync<Constellation>();

        // Seed initial data if the table is empty
        if (await _database.Table<Constellation>().CountAsync() == 0)
        {
            await SeedConstellationsFromJson();
        }
    }

    /// <summary>
    /// Seeds the database with constellation data from the JSON file
    /// </summary>
    private async Task SeedConstellationsFromJson()
    {
        try
        {
            // Open the JSON file from Resources/Raw
            using var stream = await FileSystem.OpenAppPackageFileAsync("constellations.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            // Deserialize the JSON
            var data = JsonConvert.DeserializeObject<ConstellationData>(json);

            if (data?.Constellations != null)
            {
                // Convert JSON objects to database models
                var constellations = data.Constellations.Select(c => new Constellation
                {
                    Name = c.Name,
                    Abbreviation = c.Abbreviation,
                    Description = c.Description,
                    BestVisibleMonth = c.BestVisibleMonth,
                    Hemisphere = c.Hemisphere,
                    VisibleLatitude = c.VisibleLatitude,
                    BrightestStar = c.BrightestStar,
                    NumberOfStars = c.NumberOfStars,
                    Area = c.Area,
                    // Use local packaged images in Resources/Images/Constellations by name
                    ImageUrl = $"Constellations/{c.Name.Replace(" ", "")}.png",
                    IsFavorite = false
                }).ToList();
                // Insert all into database
                await _database.InsertAllAsync(constellations);
                System.Diagnostics.Debug.WriteLine($"Successfully seeded {constellations.Count} constellations!");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding constellations: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all constellations
    /// </summary>
    public async Task<List<Constellation>> GetConstellationsAsync()
    {
        await Init();
        return await _database.Table<Constellation>().ToListAsync();
    }

    /// <summary>
    /// Search constellations by name
    /// </summary>
    public async Task<List<Constellation>> SearchConstellationsAsync(string searchTerm)
    {
        await Init();
        return await _database.Table<Constellation>()
            .Where(c => c.Name.Contains(searchTerm))
            .ToListAsync();
    }

    /// <summary>
    /// Filter constellations by hemisphere
    /// </summary>
    public async Task<List<Constellation>> GetConstellationsByHemisphereAsync(string hemisphere)
    {
        await Init();
        return await _database.Table<Constellation>()
            .Where(c => c.Hemisphere == hemisphere || c.Hemisphere == "Both")
            .ToListAsync();
    }

    /// <summary>
    /// Get all favorite constellations
    /// </summary>
    public async Task<List<Constellation>> GetFavoriteConstellationsAsync()
    {
        await Init();
        return await _database.Table<Constellation>()
            .Where(c => c.IsFavorite)
            .ToListAsync();
    }

}
