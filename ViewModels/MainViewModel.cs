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
}