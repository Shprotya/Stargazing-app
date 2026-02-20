using SQLite;
using StargazingApp.Models;

public class DatabaseService
{
    SQLiteAsyncConnection _database;

    public async Task Init()
    {
        if (_database is not null) return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Stargazing.db3");
        _database = new SQLiteAsyncConnection(dbPath);

        // 1. Create the table
        await _database.CreateTableAsync<Constellation>();

        // 2. Check if the table is already populated
        var count = await _database.Table<Constellation>().CountAsync();

        if (count == 0)
        {
            // 3. If empty, add your initial data
            var initialData = new List<Constellation>
        {
            new Constellation { Name = "Orion", Description = "The Hunter, famous for his belt.", BestVisibleMonth = "January" },
            new Constellation { Name = "Ursa Major", Description = "The Great Bear, contains the Big Dipper.", BestVisibleMonth = "April" },
            new Constellation { Name = "Cassiopeia", Description = "The Queen, recognizable by her 'W' shape.", BestVisibleMonth = "November" },
            new Constellation { Name = "Cygnus", Description = "The Swan, looks like a northern cross.", BestVisibleMonth = "September" }
        };

            await _database.InsertAllAsync(initialData);
        }
    }

    public async Task<List<Constellation>> GetConstellationsAsync()
    {
        await Init();
        return await _database.Table<Constellation>().ToListAsync();
    }

    public async Task SaveConstellationAsync(Constellation item)
    {
        await Init();
        await _database.InsertAsync(item);
    }
}