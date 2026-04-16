using StargazingApp.Models;
using StargazingApp.Services;

namespace StargazingApp.Views;

public partial class ConstellationDetailPage : ContentPage
{
    private readonly Constellation _constellation;
    private readonly DatabaseService _databaseService;

    public ConstellationDetailPage(Constellation constellation, DatabaseService databaseService)
    {
        InitializeComponent();
        _constellation = constellation;
        _databaseService = databaseService;
        BindingContext = _constellation;   // direct binding to the model
    }

    private async void ToggleFavorite_Clicked(object sender, EventArgs e)
    {
        await _databaseService.ToggleFavoriteAsync(_constellation.Id);
        // Sync the local object so the star icon updates immediately
        _constellation.IsFavorite = !_constellation.IsFavorite;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}