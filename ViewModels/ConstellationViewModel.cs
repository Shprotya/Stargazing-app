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

    public ConstellationViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task LoadAllAsync()
    {
        var items = await _databaseService.GetConstellationsAsync();
        Constellations.Clear();
        foreach (var c in items)
            Constellations.Add(c);
    }

    public async Task SearchAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            await LoadAllAsync();
            return;
        }

        var items = await _databaseService.SearchConstellationsAsync(text);
        Constellations.Clear();
        foreach (var c in items)
            Constellations.Add(c);
    }

    public async Task FilterNorthernAsync()
    {
        var items = await _databaseService.GetConstellationsAsync();
        var filtered = items
            .Where(c => c.Hemisphere == "Northern" || c.Hemisphere == "Both");
        Constellations.Clear();
        foreach (var c in filtered)
            Constellations.Add(c);
    }

    public async Task FilterSouthernAsync()
    {
        var items = await _databaseService.GetConstellationsAsync();
        var filtered = items
            .Where(c => c.Hemisphere == "Southern" || c.Hemisphere == "Both");
        Constellations.Clear();
        foreach (var c in filtered)
            Constellations.Add(c);
    }

    public async Task FilterFavoritesAsync()
    {
        var items = await _databaseService.GetFavoriteConstellationsAsync();
        Constellations.Clear();
        foreach (var c in items)
            Constellations.Add(c);
    }

    public async Task ToggleFavoriteAsync(int constellationId)
    {
        await _databaseService.ToggleFavoriteAsync(constellationId);

        // Simple: reload all, or you can reload the current filter later
        await LoadAllAsync();
    }
}
