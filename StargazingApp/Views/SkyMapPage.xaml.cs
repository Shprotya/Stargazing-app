using StargazingApp.Services;

namespace StargazingApp.Views;

public partial class SkyMapPage : ContentPage
{
    private readonly LocationService _locationService;
    private bool _locationInjected = false;

    public SkyMapPage(LocationService locationService)
    {
        InitializeComponent();
        _locationService = locationService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Only fetch location once per session
        if (_locationInjected) return;

        var location = await _locationService.GetLocationAsync();

        if (location != null)
        {
            StatusLabel.Text = $"📍 {location.Value.Lat:F3}°, {location.Value.Lon:F3}°  —  Live sky view";
            // Store coords so we can inject them once the page finishes loading
            _pendingLat = location.Value.Lat;
            _pendingLon = location.Value.Lon;
            _hasLocation = true;
        }
        else
        {
            StatusLabel.Text = "📍 Location unavailable — showing default view";
        }
    }

    private double _pendingLat;
    private double _pendingLon;
    private bool _hasLocation = false;

    private async void OnNavigated(object sender, WebNavigatedEventArgs e)
    {
        // Hide loading overlay once the page loads
        LoadingOverlay.IsVisible = false;

        if (!_hasLocation || _locationInjected) return;

        // Give Stellarium's JS a moment to fully initialise
        await Task.Delay(2500);

        // Inject coordinates via Stellarium Web's public API
        var js = $@"
            if (typeof StelWebEngine !== 'undefined') {{
                StelWebEngine.setObserverLocation({_pendingLat.ToString(System.Globalization.CultureInfo.InvariantCulture)},
                                                  {_pendingLon.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 0);
            }}
        ";

        try
        {
            await SkyWebView.EvaluateJavaScriptAsync(js);
            _locationInjected = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"JS injection failed: {ex.Message}");
            // App still works fine — Stellarium just shows its default location
        }
    }
}