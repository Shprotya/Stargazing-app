using Newtonsoft.Json;
using SQLite;
using StargazingApp.Models;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    // This runs once and sets everything up
    private async Task Init()
    {
        if (_database is not null) return; // if already set up, do nothing

        // Find where to save the file on the device
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Stargazing.db3"); 

        _database = new SQLiteAsyncConnection(dbPath); 

        // Create the Constellations table if it doesn't exist
        await _database.CreateTableAsync<Constellation>(); 

        // This handles the automatic update check every time the app starts
        await RefreshDatabaseIfChanged();
    }

    private async Task RefreshDatabaseIfChanged()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("constellations.json"); 
            using var reader = new StreamReader(stream); 
            var json = await reader.ReadToEndAsync(); 
            var data = JsonConvert.DeserializeObject<ConstellationData>(json);

            if (data?.Constellations == null) return; 

            // Get the version currently saved on the device (default to 0 if new install)
            int currentDbVersion = Preferences.Default.Get("database_version", 0);
            var dbCount = await _database.Table<Constellation>().CountAsync(); 

            // Trigger update if version is higher OR if count is different
            if (data.Version > currentDbVersion || dbCount != data.Constellations.Count)
            {
                // Remember which constellations were favorites
                var existingFavorites = await _database.Table<Constellation>()
                                                 .Where(c => c.IsFavorite)
                                                 .ToListAsync(); 
                var favoriteNames = existingFavorites.Select(f => f.Name).ToList();

                // Clear and rebuild the table
                await _database.DeleteAllAsync<Constellation>();

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
                    ImageUrl = $"Resources/Images/Constellations/{c.Name.Replace(" ", "")}.png", 
                    // 3. Restore favorite status if it was previously favorited
                    IsFavorite = favoriteNames.Contains(c.Name)
                }).ToList();

                await _database.InsertAllAsync(constellations); 

                // Save the new version number to Preferences
                Preferences.Default.Set("database_version", data.Version);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}"); 
        }
    }

    // Filter & Search Methods

    public async Task<List<Constellation>> GetConstellationsAsync()
    {
        await Init(); 
        return await _database.Table<Constellation>().ToListAsync(); 
    }

    public async Task<List<Constellation>> SearchConstellationsAsync(string searchTerm)
    {
        await Init(); 
        return await _database.Table<Constellation>()
            .Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()))
            .ToListAsync(); 
    }

    public async Task<List<Constellation>> GetConstellationsByHemisphereAsync(string hemisphere)
    {
        await Init(); 
        return await _database.Table<Constellation>()
            .Where(c => c.Hemisphere == hemisphere || c.Hemisphere == "Both") 
            .ToListAsync(); 
    }

    public async Task<List<Constellation>> GetFavoriteConstellationsAsync()
    {
        await Init(); 
        return await _database.Table<Constellation>()
            .Where(c => c.IsFavorite) 
            .ToListAsync(); 
    }

    public async Task ToggleFavoriteAsync(int constellationId)
    {
        await Init(); 
        var constellation = await _database.Table<Constellation>()
            .FirstOrDefaultAsync(c => c.Id == constellationId); 

        if (constellation != null)
        {
            constellation.IsFavorite = !constellation.IsFavorite; 
            await _database.UpdateAsync(constellation); 
        }
    }
}