using SQLite;
using StargazingApp.Models;
using Newtonsoft.Json;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    // This runs once and sets everything up
    private async Task Init()
    {
        if (_database is not null) return; // is already set up, do nothing

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
                    ImageUrl = $"Resources/Images/Constellations/{c.Name}.png",
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
    /// Filter constellations by best viewing month
    /// </summary>
    public async Task<List<Constellation>> GetConstellationsByMonthAsync(string month)
    {
        await Init();
        return await _database.Table<Constellation>()
            .Where(c => c.BestVisibleMonth == month)
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

    /// <summary>
    /// Save or update a constellation
    /// </summary>
    public async Task SaveConstellationAsync(Constellation item)
    {
        await Init();

        if (item.Id != 0)
        {
            // Update existing
            await _database.UpdateAsync(item);
        }
        else
        {
            // Insert new
            await _database.InsertAsync(item);
        }
    }

    /// <summary>
    /// Toggle favorite status for a constellation
    /// </summary>
    public async Task ToggleFavoriteAsync(int constellationId)
    {
        await Init();
        var constellation = await _database.Table<Constellation>()
            .Where(c => c.Id == constellationId)
            .FirstOrDefaultAsync();

        if (constellation != null)
        {
            constellation.IsFavorite = !constellation.IsFavorite;
            await _database.UpdateAsync(constellation);
        }
    }

    /// <summary>
    /// Delete a constellation
    /// </summary>
    public async Task DeleteConstellationAsync(Constellation item)
    {
        await Init();
        await _database.DeleteAsync(item);
    }

    /// <summary>
    /// Clear all data (useful for testing/debugging)
    /// </summary>
    public async Task ClearAllDataAsync()
    {
        await Init();
        await _database.DeleteAllAsync<Constellation>();
    }
}