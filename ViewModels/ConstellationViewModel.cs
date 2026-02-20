using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System.Collections.ObjectModel;

public partial class ConstellationViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    // This is the list the UI will show
    public ObservableCollection<Constellation> Constellations { get; set; } = new();

    public ConstellationViewModel(DatabaseService db)
    {
        _db = db;
    }

    [RelayCommand]
    async Task LoadConstellations()
    {
        var items = await _db.GetConstellationsAsync();
        Constellations.Clear();
        foreach (var item in items)
            Constellations.Add(item);
    }
}