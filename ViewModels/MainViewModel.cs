using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System;
using System.Threading.Tasks;

namespace StargazingApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly NasaApiService _nasaService;

    [ObservableProperty]
    private ApodData _apodData;

    // This will control the spinner
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    public MainViewModel(NasaApiService nasaService)
    {
        _nasaService = nasaService;
        LoadApodCommand = new AsyncRelayCommand(LoadApod);
    }

    public IAsyncRelayCommand LoadApodCommand { get; }

    private async Task LoadApod()
    {
        try
        {
            IsBusy = true; // Start the spinner
            HasError = false; // Reset error state at start
            ErrorMessage = string.Empty;

            var result = await _nasaService.GetApodAsync();

            if (result != null)
            {
                ApodData = result;
            }
            else
            {
                HasError = true;
                ErrorMessage = "Could not find today's data. Space might be empty today!";
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false; // Stop the spinner even if it fails
        }
    }
}
