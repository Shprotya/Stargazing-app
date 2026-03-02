using StargazingApp.Models;

namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private List<Constellation> _allConstellations = new();

    public ConstellationPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAllConstellations();
    }

    /// <summary>
    /// Load all constellations from database
    /// </summary>
    private async Task LoadAllConstellations()
    {
        try
        {
            _allConstellations = await _databaseService.GetConstellationsAsync();
            ConstellationList.ItemsSource = _allConstellations;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading constellations: {ex.Message}");
            await DisplayAlert("Error", "Failed to load constellations", "OK");
        }
    }

    /// <summary>
    /// Search button pressed
    /// </summary>
    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        await PerformSearch();
    }

    /// <summary>
    /// Search text changed (real-time search)
    /// </summary>
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            // If search is cleared, show all
            ConstellationList.ItemsSource = _allConstellations;
        }
        else
        {
            await PerformSearch();
        }
    }

    /// <summary>
    /// Perform the search
    /// </summary>
    private async Task PerformSearch()
    {
        var searchTerm = SearchBar.Text;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            ConstellationList.ItemsSource = _allConstellations;
            return;
        }

        try
        {
            var results = await _databaseService.SearchConstellationsAsync(searchTerm);
            ConstellationList.ItemsSource = results;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
        }
    }

    /// <summary>
    /// Filter: Show all constellations
    /// </summary>
    private void OnFilterAll(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        ConstellationList.ItemsSource = _allConstellations;
        SearchBar.Text = string.Empty;
    }

    /// <summary>
    /// Filter: Show only Northern hemisphere constellations
    /// </summary>
    private async void OnFilterNorthern(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        try
        {
            var results = await _databaseService.GetConstellationsByHemisphereAsync("Northern");
            ConstellationList.ItemsSource = results;
            SearchBar.Text = string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Filter error: {ex.Message}");
        }
    }

    /// <summary>
    /// Filter: Show only Southern hemisphere constellations
    /// </summary>
    private async void OnFilterSouthern(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        try
        {
            var results = await _databaseService.GetConstellationsByHemisphereAsync("Southern");
            ConstellationList.ItemsSource = results;
            SearchBar.Text = string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Filter error: {ex.Message}");
        }
    }

    /// <summary>
    /// Filter: Show only favorite constellations
    /// </summary>
    private async void OnFilterFavorites(object sender, EventArgs e)
    {
        UpdateButtonColors(sender as Button);
        try
        {
            var results = await _databaseService.GetFavoriteConstellationsAsync();
            ConstellationList.ItemsSource = results;
            SearchBar.Text = string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Filter error: {ex.Message}");
        }
    }

    /// <summary>
    /// Toggle favorite status when star button is clicked
    /// </summary>
    private async void OnFavoriteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int constellationId)
        {
            try
            {
                // Toggle in database
                await _databaseService.ToggleFavoriteAsync(constellationId);

                // Reload all constellations from database
                _allConstellations = await _databaseService.GetConstellationsAsync();

                // Refresh the current view to show updated star
                var currentItems = ConstellationList.ItemsSource as List<Constellation>;
                if (currentItems != null)
                {
                    // Recreate the list with updated data
                    var updatedItems = _allConstellations
                        .Where(c => currentItems.Any(ci => ci.Id == c.Id))
                        .ToList();

                    ConstellationList.ItemsSource = null;
                    ConstellationList.ItemsSource = updatedItems;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Favorite toggle error: {ex.Message}");
            }
        }
    }

    private void UpdateButtonColors(Button selectedButton)
    {
        // Reset all buttons to the default color
        BtnAll.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnNorth.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnSouth.BackgroundColor = (Color)Application.Current.Resources["Button"];
        BtnFav.BackgroundColor = (Color)Application.Current.Resources["Button"];

        // Highlight the selected one
        selectedButton.BackgroundColor = (Color)Application.Current.Resources["NovaPurple"];
    }
}