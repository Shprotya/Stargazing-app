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
        ConstellationList.ItemsSource = _allConstellations;
        SearchBar.Text = string.Empty;
    }

    /// <summary>
    /// Filter: Show only Northern hemisphere constellations
    /// </summary>
    private async void OnFilterNorthern(object sender, EventArgs e)
    {
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
                await _databaseService.ToggleFavoriteAsync(constellationId);

                // Refresh the current view
                if (ConstellationList.ItemsSource is List<Constellation> currentList)
                {
                    var constellation = currentList.FirstOrDefault(c => c.Id == constellationId);
                    if (constellation != null)
                    {
                        constellation.IsFavorite = !constellation.IsFavorite;

                        // Force UI refresh
                        var temp = ConstellationList.ItemsSource;
                        ConstellationList.ItemsSource = null;
                        ConstellationList.ItemsSource = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Favorite toggle error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Handle constellation selection for detail view (optional - you can expand this later)
    /// </summary>
    private async void OnConstellationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Constellation constellation)
        {
            // For now, just show an alert with details
            await DisplayAlert(
                constellation.Name,
                $"Abbreviation: {constellation.Abbreviation}\n" +
                $"Hemisphere: {constellation.Hemisphere}\n" +
                $"Best Month: {constellation.BestVisibleMonth}\n" +
                $"Brightest Star: {constellation.BrightestStar}\n" +
                $"Visible: {constellation.VisibleLatitude}\n\n" +
                $"{constellation.Description}",
                "OK"
            );

            // Deselect the item
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}