using StargazingApp.Models;

namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    private readonly ConstellationViewModel _viewModel;

    public ConstellationPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _viewModel = new ConstellationViewModel(databaseService);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAllAsync();
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        await _viewModel.SearchAsync(SearchBar.Text);
    }

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        await _viewModel.SearchAsync(e.NewTextValue);
    }

    private async void OnFilterAll(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        await _viewModel.LoadAllAsync();
        SearchBar.Text = string.Empty;
    }

    private async void OnFilterNorthern(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        await _viewModel.FilterNorthernAsync();
        SearchBar.Text = string.Empty;
    }

    private async void OnFilterSouthern(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        await _viewModel.FilterSouthernAsync();
        SearchBar.Text = string.Empty;
    }

    private async void OnFilterFavorites(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        await _viewModel.FilterFavoritesAsync();
        SearchBar.Text = string.Empty;
    }

    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int constellationId)
        {
            await _viewModel.ToggleFavoriteAsync(constellationId);
        }
    }

    private void UpdateButtonColors(Button selectedButton)
    {
        BtnAll.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnNorth.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnSouth.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnFav.BackgroundColor = (Color)Application.Current.Resources["Button"];

        selectedButton.BackgroundColor = (Color)Application.Current.Resources["NovaPurple"];
    }
}