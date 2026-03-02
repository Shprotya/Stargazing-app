using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using StargazingApp.Services;

namespace StargazingApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly NasaApiService _nasaService;
    private readonly LocationService _locationService;
    private readonly SevenTimerService _sevenTimerService;

    [ObservableProperty] private ApodData apodData;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage;
    [ObservableProperty] private bool hasError;

    // Moon
    [ObservableProperty] private string moonPhaseIcon;
    [ObservableProperty] private string moonPhaseName;
    [ObservableProperty] private string moonIllumination;

    // Date/Time
    [ObservableProperty] private string currentDateTime;

    // Visibility
    [ObservableProperty] private string visibilityRating = "Loading...";
    [ObservableProperty] private string locationName = "";

    public IAsyncRelayCommand LoadApodCommand { get; }

    public MainViewModel(NasaApiService nasaService, LocationService locationService, SevenTimerService sevenTimerService)
    {
        _nasaService = nasaService;
        _locationService = locationService;
        _sevenTimerService = sevenTimerService;
        LoadApodCommand = new AsyncRelayCommand(LoadAll);
    }

    private async Task LoadAll()
    {
        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        CalculateMoonPhase();

        // Run NASA and visibility in parallel
        await Task.WhenAll(LoadApodAsync(), LoadVisibilityAsync());

        IsBusy = false;
    }

    private async Task LoadApodAsync()
    {
        try
        {
            var result = await _nasaService.GetApodAsync();
            if (result != null)
                ApodData = result;
            else
            {
                HasError = true;
                ErrorMessage = "Could not load NASA picture.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error: {ex.Message}";
        }
    }

    private async Task LoadVisibilityAsync()
    {
        var location = await _locationService.GetLocationAsync();
        if (location == null)
        {
            VisibilityRating = "📍 Location unavailable";
            return;
        }

        var conditions = await _sevenTimerService.GetCurrentConditionsAsync(location.Value.Lat, location.Value.Lon);
        if (conditions == null)
        {
            VisibilityRating = "⚠️ Could not load forecast";
            return;
        }

        VisibilityRating = _sevenTimerService.GetVisibilityRating(conditions);
    }

    private void CalculateMoonPhase()
    {
        CurrentDateTime = DateTime.Now.ToString("dddd, MMM dd • HH:mm");

        var knownNewMoon = new DateTime(2000, 1, 6, 18, 14, 0);
        var daysSince = (DateTime.UtcNow - knownNewMoon).TotalDays;
        var synodicMonth = 29.53058867;
        var phase = daysSince % synodicMonth;
        var illumination = (int)(50 * (1 - Math.Cos(2 * Math.PI * phase / synodicMonth)));

        MoonIllumination = $"Illumination: {illumination}%";

        (MoonPhaseIcon, MoonPhaseName) = phase switch
        {
            < 1.85 => ("🌑", "New Moon"),
            < 7.38 => ("🌒", "Waxing Crescent"),
            < 9.22 => ("🌓", "First Quarter"),
            < 14.77 => ("🌔", "Waxing Gibbous"),
            < 16.61 => ("🌕", "Full Moon"),
            < 22.15 => ("🌖", "Waning Gibbous"),
            < 23.99 => ("🌗", "Last Quarter"),
            _ => ("🌘", "Waning Crescent")
        };
    }
}