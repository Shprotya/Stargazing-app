using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using StargazingApp.Services;
using System.Diagnostics;

namespace StargazingApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private System.Threading.Timer _clockTimer;

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
    [ObservableProperty] private string sunriseTime;
    [ObservableProperty] private string sunsetTime;

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

        _clockTimer = new System.Threading.Timer(_ =>
        {
            MainThread.BeginInvokeOnMainThread(() => CurrentDateTime = DateTime.Now.ToString("dddd, MMM dd • HH:mm"));
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
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
            SunriseTime = "--:--";
            SunsetTime = "--:--";
            return;
        }

        var (sunrise, sunset) = CalculateSunTimes(location.Value.Lat, location.Value.Lon);
        var now = DateTime.Now;
        var isDaytime = now >= sunrise && now <= sunset;

        SunriseTime = sunrise.ToString("HH:mm");
        SunsetTime = sunset.ToString("HH:mm");

        var conditions = await _sevenTimerService.GetCurrentConditionsAsync(location.Value.Lat, location.Value.Lon);
        if (conditions == null)
        {
            VisibilityRating = "⚠️ Could not load forecast";
            return;
        }

        //Debugging and location check
        //VisibilityRating = $"📍 Got location: {location.Value.Lat:F3}, {location.Value.Lon:F3}";

        var rating = _sevenTimerService.GetVisibilityRating(conditions);
        VisibilityRating = isDaytime
            ? $"{rating}\n☀️ It's daytime — check back after sunset."
            : rating;
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

    private (DateTime Sunrise, DateTime Sunset) CalculateSunTimes(double lat, double lon)
    {
        var now = DateTime.UtcNow;
        var dayOfYear = now.DayOfYear;

        // Solar declination and equation of time
        var declination = 23.45 * Math.Sin((360.0 / 365.0 * (dayOfYear - 81)) * Math.PI / 180.0);
        var latRad = lat * Math.PI / 180.0;
        var declRad = declination * Math.PI / 180.0;

        // Hour angle at sunrise/sunset
        var cosHourAngle = -Math.Tan(latRad) * Math.Tan(declRad);

        // Clamp to valid range (handles midnight sun / polar night)
        cosHourAngle = Math.Clamp(cosHourAngle, -1.0, 1.0);

        var hourAngle = Math.Acos(cosHourAngle) * 180.0 / Math.PI;

        // Solar noon (adjusted for longitude and equation of time)
        var solarNoon = 12.0 - (lon / 15.0);
        var sunriseUtc = solarNoon - (hourAngle / 15.0);
        var sunsetUtc = solarNoon + (hourAngle / 15.0);

        // Convert to local time
        var offset = TimeZoneInfo.Local.GetUtcOffset(now).TotalHours;
        var sunrise = now.Date.AddHours(sunriseUtc + offset);
        var sunset = now.Date.AddHours(sunsetUtc + offset);

        return (sunrise, sunset);
    }
}