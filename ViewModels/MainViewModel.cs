using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StargazingApp.Models;
using System;
using System.Threading.Tasks;

namespace StargazingApp.ViewModels;

/// <summary>
/// ViewModel for the Main Page of the Stargazing App.
/// Implements the MVVM pattern to separate UI logic from the View.
/// Handles data retrieval from NASA API and exposes properties for data binding.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    // Service dependency for NASA API calls
    private readonly NasaApiService _nasaService;

    /// <summary>
    /// Contains the current APOD (Astronomy Picture of the Day) data.
    /// This property is bound to UI elements in MainPage.xaml.
    /// The [ObservableProperty] attribute automatically generates the backing field
    /// and implements INotifyPropertyChanged to update the UI when the value changes.
    /// </summary>
    [ObservableProperty]
    private ApodData _apodData;

    /// <summary>
    /// Indicates whether the app is currently loading data from the API.
    /// </summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>
    /// Contains any error message that occurs during API calls.
    /// </summary>
    [ObservableProperty]
    private string _errorMessage;

    /// <summary>
    /// Indicates whether an error has occurred.
    /// </summary>
    [ObservableProperty]
    private bool _hasError;

    /// <summary>
    /// Constructor that injects the NasaApiService dependency.
    /// Initializes the LoadApodCommand for use in the UI.
    /// </summary>
    /// <param name="nasaService">The NASA API service for fetching APOD data.</param>
    public MainViewModel(NasaApiService nasaService)
    {
        _nasaService = nasaService;

        // Initialize the async command that will be triggered from the UI
        LoadApodCommand = new AsyncRelayCommand(LoadApod);
    }

    /// <summary>
    /// Command that can be bound to UI elements (like buttons) to trigger the LoadApod method.
    /// AsyncRelayCommand handles async operations and automatically disables the command
    /// while it's executing to prevent multiple simultaneous calls.
    /// </summary>
    public IAsyncRelayCommand LoadApodCommand { get; }

    /// <summary>
    /// Asynchronously loads today's Astronomy Picture of the Day from NASA's API.
    /// This method handles all states: loading, success, and error conditions.
    /// Updates the UI through bound properties (IsBusy, HasError, ErrorMessage, ApodData).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LoadApod()
    {
        try
        {
            // Set IsBusy to true - this shows the loading spinner in the UI
            IsBusy = true;

            // Reset error state at the start of a new request
            HasError = false;
            ErrorMessage = string.Empty;

            // Call the NASA API service to fetch today's APOD
            var result = await _nasaService.GetApodAsync();

            // Check if we received valid data
            if (result != null)
            {
                // Update the ApodData property
                ApodData = result;
            }
            else
            {
                // No data received - set error state
                HasError = true;
                ErrorMessage = "Could not find today's data. Space might be empty today!";
            }
        }
        catch (Exception ex)
        {
            // Catch any unexpected errors (network issues, parsing errors, etc.)
            HasError = true;
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            // Stop the loading spinner
            // The finally block ensures this runs even if an exception occurs
            IsBusy = false;
        }
    }
}