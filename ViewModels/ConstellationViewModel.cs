using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System.Collections.ObjectModel;

public partial class ConstellationViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Constellation> constellations = new();

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private string activeFilter = "All";

    public ConstellationViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task LoadAllAsync()
    {
        ActiveFilter = "All"; // This triggers button color update via PropertyChanged
        var items = await _databaseService.GetConstellationsAsync();
        UpdateList(items.OrderBy(c => c.Name)); // Sort by name
    }

    [RelayCommand]
    public async Task FilterByHemisphereAsync(string hemisphere)
    {
        ActiveFilter = hemisphere; // Set state (Northern or Southern)

        // Get constellations that are ONLY in the specified hemisphere (exclude "Both")
        var allItems = await _databaseService.GetConstellationsAsync();
        var filtered = allItems.Where(c => c.Hemisphere == hemisphere).OrderBy(c => c.Name);

        UpdateList(filtered);
    }

    [RelayCommand]
    public async Task FilterFavoritesAsync()
    {
        ActiveFilter = "Favorites";
        var items = await _databaseService.GetFavoriteConstellationsAsync();
        UpdateList(items.OrderBy(c => c.Name)); // Sort by name
    }

    [RelayCommand]
    public async Task SearchAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            await LoadAllAsync();
            return;
        }

        var items = await _databaseService.SearchConstellationsAsync(text);
        UpdateList(items.OrderBy(c => c.Name)); // Sort by name
    }

    [RelayCommand]
    public async Task ToggleFavoriteAsync(int constellationId)
    {
        await _databaseService.ToggleFavoriteAsync(constellationId);

        // Reload the current view to show updated star
        switch (ActiveFilter)
        {
            case "All":
                await LoadAllAsync();
                break;
            case "Northern":
                await FilterByHemisphereAsync("Northern");
                break;
            case "Southern":
                await FilterByHemisphereAsync("Southern");
                break;
            case "Favorites":
                await FilterFavoritesAsync();
                break;
        }
    }

    // Helper to avoid repeating Clear/Add logic
    private void UpdateList(IEnumerable<Constellation> items)
    {
        Constellations.Clear();
        foreach (var c in items)
            Constellations.Add(c);
    }
}