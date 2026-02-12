using Newtonsoft.Json;
using StargazingApp.Models;
using System.Net.Http.Json;

namespace StargazingApp;

public class NasaApiService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "9HdK398eTbD3leLSbVGYwV1YpetcNNWtpClTv5Qw";
    private const string BaseUrl = "https://api.nasa.gov/planetary/apod";

    public NasaApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<ApodData?> GetApodAsync()
    {
        try
        {
            var url = $"{BaseUrl}?api_key={ApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
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
            System.Diagnostics.Debug.WriteLine($"Error fetching APOD: {ex.Message}");
            return null;
        }
    }

    public async Task<ApodData?> GetApodByDateAsync(DateTime date)
    {
        try
        {
            var dateString = date.ToString("yyyy-MM-dd");
            var url = $"{BaseUrl}?api_key={ApiKey}&date={dateString}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
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
            System.Diagnostics.Debug.WriteLine($"Error fetching APOD: {ex.Message}");
            return null;
        }
    }
}
