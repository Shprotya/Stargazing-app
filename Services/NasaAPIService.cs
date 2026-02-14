using Newtonsoft.Json;
using StargazingApp.Models;
using System.Net.Http.Json;

namespace StargazingApp;

/// <summary>
/// Service class responsible for handling all NASA API communications.
/// Provides methods to fetch Astronomy Picture of the Day (APOD) data.
/// </summary>
public class NasaApiService
{
    // HttpClient for making HTTP requests to the NASA API
    private readonly HttpClient _httpClient;

    private const string ApiKey = "9HdK398eTbD3leLSbVGYwV1YpetcNNWtpClTv5Qw";
    private const string BaseUrl = "https://api.nasa.gov/planetary/apod";

    public NasaApiService()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Fetches today's Astronomy Picture of the Day from NASA's API.
    /// </summary>
    /// <returns>
    /// An ApodData object containing today's picture information, or null if the request fails.
    /// </returns>
    public async Task<ApodData?> GetApodAsync()
    {
        try
        {
            // Construct the API URL with the authentication key
            var url = $"{BaseUrl}?api_key={ApiKey}";

            // Send GET request to NASA API
            var response = await _httpClient.GetAsync(url);

            // Check if the request was successful (HTTP 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Read the JSON response as a string
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an ApodData object
                var apodData = JsonConvert.DeserializeObject<ApodData>(jsonString);
                return apodData;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions to the debug console for troubleshooting
            System.Diagnostics.Debug.WriteLine($"Error fetching APOD: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Fetches the Astronomy Picture of the Day for a specific date from NASA's API.
    /// </summary>
    /// <param name="date">The date for which to retrieve the APOD data.</param>
    /// <returns>
    /// An ApodData object containing the picture information for the specified date, 
    /// or null if the request fails.
    /// </returns>
    public async Task<ApodData?> GetApodByDateAsync(DateTime date)
    {
        try
        {
            // Format the date in the required format (yyyy-MM-dd) for the API
            var dateString = date.ToString("yyyy-MM-dd");

            // Construct the API URL
            var url = $"{BaseUrl}?api_key={ApiKey}&date={dateString}";

            // Send GET request to NASA API
            var response = await _httpClient.GetAsync(url);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the JSON response as a string
                var jsonString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into an ApodData object
                var apodData = JsonConvert.DeserializeObject<ApodData>(jsonString);
                return apodData;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            // Log any exceptions to the debug console for troubleshooting
            System.Diagnostics.Debug.WriteLine($"Error fetching APOD: {ex.Message}");
            return null;
        }
    }
}