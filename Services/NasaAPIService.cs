using Newtonsoft.Json.Linq;
using System.Net.Http;

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

    // return 'dynamic' so we can access properties on the fly
    public async Task<dynamic?> GetApodAsync()
    {
        try
        {
            var url = $"{BaseUrl}?api_key={ApiKey}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                dynamic data = JObject.Parse(jsonString);
                return data;
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }
}