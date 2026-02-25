using SQLite;
using StargazingApp.Models;

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
            await _database.InsertAllAsync(new List<Constellation>
            {
                new() { Name = "Orion",      Description = "The Hunter, famous for his belt.",          BestVisibleMonth = "January"  },
                new() { Name = "Ursa Major", Description = "The Great Bear, contains the Big Dipper.", BestVisibleMonth = "April"    },
                new() { Name = "Cassiopeia", Description = "The Queen, recognizable by her W shape.",  BestVisibleMonth = "November" },
                new() { Name = "Cygnus",     Description = "The Swan, looks like a northern cross.",   BestVisibleMonth = "September"}
            });
        }

    }

    // Get all constellations
    public async Task<List<Constellation>> GetConstellationsAsync()
    {
        await Init();
        System.Diagnostics.Debug.WriteLine($"DB Path: {Path.Combine(FileSystem.AppDataDirectory, "Stargazing.db3")}");
        return await _database.Table<Constellation>().ToListAsync();
    }

    // Save a new constellation
    public async Task SaveConstellationAsync(Constellation item)
    {
        await Init();
        await _database.InsertAsync(item);
    }
}