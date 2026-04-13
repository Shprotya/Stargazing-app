using Newtonsoft.Json;
using SQLite;
using StargazingApp.Models;
using System.Diagnostics;

namespace StargazingApp.Services;
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
        // and the JournalEntries table for user logs.
        await _database.CreateTableAsync<Constellation>();
        await _database.CreateTableAsync<JournalEntry>();

        // This handles the automatic update check every time the app starts
        await RefreshDatabaseIfChanged();
    }

    private async Task RefreshDatabaseIfChanged()
    {
        try
        {
            // 1. Load the JSON from the app package
            using var stream = await FileSystem.OpenAppPackageFileAsync("constellations.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ConstellationData>(json);

            if (data?.Constellations == null) return;

            // 2. Check versions
            int currentDbVersion = Preferences.Default.Get("database_version", 0);

            // Only update if the JSON version is higher OR the database is empty
            var dbCount = await _database.Table<Constellation>().CountAsync();

            if (data.Version > currentDbVersion || dbCount == 0)
            {
                // 3. Keep track of user favorites before wiping!
                // We don't want the user to lose their "Star" selections just because we updated the data.
                var favorites = await _database.Table<Constellation>()
                                               .Where(c => c.IsFavorite)
                                               .ToListAsync();
                var favoriteNames = favorites.Select(f => f.Name).ToList();

                // 4. Wipe the existing table to prevent "Ghost" records
                await _database.DeleteAllAsync<Constellation>();

                // 5. Transform and Insert new data
                var newItems = data.Constellations.Select(jsonItem => new Constellation
                {
                    Name = jsonItem.Name,
                    Abbreviation = jsonItem.Abbreviation,
                    Description = jsonItem.Description,
                    BestVisibleMonth = jsonItem.BestVisibleMonth,
                    Hemisphere = jsonItem.Hemisphere.Trim(), // Clean whitespace
                    VisibleLatitude = jsonItem.VisibleLatitude,
                    BrightestStar = jsonItem.BrightestStar,
                    NumberOfStars = jsonItem.NumberOfStars,
                    Area = jsonItem.Area,
                    ImageUrl = $"Resources/Images/Constellations/{jsonItem.ImageUrl}",
                    // Restore favorite status if the name matches
                    IsFavorite = favoriteNames.Contains(jsonItem.Name)
                }).ToList();

                await _database.InsertAllAsync(newItems);

                // 6. Update the saved version number
                Preferences.Default.Set("database_version", data.Version);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Database Refresh Failed: {ex.Message}");
        }
    }

    #region Filter & Search Methods
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

        // Use SQL-level filtering for performance.
        // 'Both' is included so constellations visible in both show up in either filter.
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

    #endregion

    #region Journal Methods
    public async Task<List<JournalEntry>> GetJournalEntriesAsync()
    {
        await Init();
        // LINQ: order by most recent first
        var entries = await _database.Table<JournalEntry>().ToListAsync();
        return entries.OrderByDescending(e => e.CreatedAt).ToList();
    }

    public async Task<List<JournalEntry>> SearchJournalAsync(string term)
    {
        await Init();
        var all = await _database.Table<JournalEntry>().ToListAsync();

        // LINQ + ISearchable interface used together
        return all
            .Where(e => e.MatchesSearch(term))
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }

    public async Task<List<JournalEntry>> GetJournalEntriesByDateRangeAsync(DateTime from, DateTime to)
    {
        await Init();
        // Working with Dates: filter by date range using LINQ
        var all = await _database.Table<JournalEntry>().ToListAsync();
        return all
            .Where(e => e.CreatedAt.Date >= from.Date && e.CreatedAt.Date <= to.Date)
            .OrderByDescending(e => e.CreatedAt)
            .ToList();
    }

    public async Task<int> SaveJournalEntryAsync(JournalEntry entry)
    {
        await Init();
        entry.UpdatedAt = DateTime.Now;

        if (entry.Id == 0)
        {
            entry.CreatedAt = DateTime.Now;
            return await _database.InsertAsync(entry);
        }
        return await _database.UpdateAsync(entry);
    }

    public async Task<int> DeleteJournalEntryAsync(JournalEntry entry)
    {
        await Init();
        return await _database.DeleteAsync(entry);
    }
    #endregion
}