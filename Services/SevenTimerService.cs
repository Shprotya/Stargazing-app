using Newtonsoft.Json;
using StargazingApp.Models;

namespace StargazingApp.Services;

public class SevenTimerService
{
    private readonly HttpClient _httpClient = new();

    public async Task<SevenTimerEntry?> GetCurrentConditionsAsync(double lat, double lon)
    {
        try
        {
            var url = $"https://www.7timer.info/bin/api.pl?lon={lon}&lat={lat}&product=astro&output=json";
            var json = await _httpClient.GetStringAsync(url);
            var data = JsonConvert.DeserializeObject<SevenTimerResponse>(json);

            // First entry = closest forecast to now
            return data?.DataSeries?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"7Timer error: {ex.Message}");
            return null;
        }
    }

    public string GetVisibilityRating(SevenTimerEntry entry)
    {
        // CloudCover: 1=0-6%, 2=6-19% ... 9=94-100%
        // Seeing & Transparency: 1=best, 8=worst
        if (entry.CloudCover <= 2 && entry.Seeing <= 3 && entry.Transparency <= 3)
            return "🟢 Excellent";
        if (entry.CloudCover <= 4 && entry.Seeing <= 5)
            return "🟡 Good";
        if (entry.CloudCover <= 6)
            return "🟠 Poor";

        return "🔴 Not visible tonight";
    }
}