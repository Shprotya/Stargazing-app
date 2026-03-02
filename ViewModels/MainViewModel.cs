using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System;
using System.Threading.Tasks;

namespace StargazingApp.ViewModels;

/// <summary>
/// Manages the logic for the Main Page, handling NASA APOD data retrieval and state.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly NasaApiService _nasaService;

    [ObservableProperty]
    private ApodData _apodData;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string moonPhaseIcon;

    [ObservableProperty]
    private string moonPhaseName;

    [ObservableProperty]
    private string moonIllumination;

    [ObservableProperty]
    private string currentDateTime;

    public IAsyncRelayCommand LoadApodCommand { get; }

    public MainViewModel(NasaApiService nasaService)
    {
        _nasaService = nasaService;
        LoadApodCommand = new AsyncRelayCommand(LoadApod);
    }

    /// <summary>
    /// Fetches the latest Astronomy Picture of the Day and updates the UI state.
    /// </summary>
    private async Task LoadApod()
    {
        CalculateMoonPhase();

        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var result = await _nasaService.GetApodAsync();

            if (result != null)
            {
                ApodData = result;
            }
            else
            {
                HasError = true;
                ErrorMessage = "Could not find today's data.";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CalculateMoonPhase()
    {
        CurrentDateTime = DateTime.Now.ToString("dddd, MMM dd • HH:mm");

        // Known new moon reference date
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
            < 29.53 => ("🌘", "Waning Crescent"),
            _ => ("🌑", "New Moon")
        };
    }
}