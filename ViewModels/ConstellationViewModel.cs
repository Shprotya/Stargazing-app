using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System.Collections.ObjectModel;

public partial class ConstellationViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    ObservableCollection<Constellation> constellations = new();

    [ObservableProperty]
    string searchText;

    [ObservableProperty]
    string activeFilter = "All"; 

    public ConstellationViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }


    [RelayCommand]
    public async Task LoadAllAsync()
    {
        activeFilter = "All"; // Set state
        var items = await _databaseService.GetConstellationsAsync();
        UpdateList(items);
    }

    [RelayCommand]
    public async Task FilterByHemisphereAsync(string hemisphere)
    {
        activeFilter = hemisphere; // Set state (Northern or Southern)
        var items = await _databaseService.GetConstellationsByHemisphereAsync(hemisphere);
        UpdateList(items);
    }

    [RelayCommand]
    public async Task FilterFavoritesAsync()
    {
        var items = await _databaseService.GetFavoriteConstellationsAsync();
        UpdateList(items);
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
        UpdateList(items);
    }

    [RelayCommand]
    public async Task ToggleFavoriteAsync(int constellationId)
    {
        await _databaseService.ToggleFavoriteAsync(constellationId);
        // Refresh the current view to show the star change
        await SearchAsync(SearchText);
    }

    // Helper to avoid repeating Clear/Add logic
    private void UpdateList(IEnumerable<Constellation> items)
    {
        Constellations.Clear();
        foreach (var c in items)
            Constellations.Add(c);
    }
}