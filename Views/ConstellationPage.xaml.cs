namespace StargazingApp.Views;

public partial class ConstellationPage : ContentPage
{
    // Declare the field at the class level
    private readonly DatabaseService _databaseService;

    // Constructor - This is where you receive the service
    public ConstellationPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    // The Logic - Must be inside an 'async' method
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Call the service to get data
            var constellations = await _databaseService.GetConstellationsAsync();

            // Assign it to the CollectionView's name from XAML
            ConstellationList.ItemsSource = constellations;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
    }
}