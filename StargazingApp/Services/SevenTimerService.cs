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

            if (data?.DataSeries == null) return null;

            // Find the entry closest to the current UTC hour
            var closest = data.DataSeries
                .OrderBy(e => e.Timepoint)
                .FirstOrDefault();

            return closest;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"7Timer error: {ex.Message}");
            return null;
        }
    }

    public string GetVisibilityRating(SevenTimerEntry entry)
    {
        string rating;

        // Seeing or Transparency being poor should downgrade the overall rating
        bool atmospherePoor = entry.Seeing >= 5 || entry.Transparency >= 5;

        if (entry.CloudCover >= 7)
            rating = $"🔴 Too cloudy...";
        else if (entry.CloudCover >= 5 || (entry.CloudCover >= 3 && atmospherePoor))
            rating = "🟠 Poor";
        else if (entry.CloudCover >= 3 || atmospherePoor)
            rating = "🟡 Fair";
        else if (entry.CloudCover <= 2 && entry.Seeing <= 3 && entry.Transparency <= 3)
            rating = "🟢 Excellent";
        else
            rating = "🟡 Fair";

        // Human-readable breakdown
        var cloudText = entry.CloudCover switch
        {
            1 => "0–6%",
            2 => "6–19%",
            3 => "19–31%",
            4 => "31–44%",
            5 => "44–56%",
            6 => "56–69%",
            7 => "69–81%",
            8 => "81–94%",
            9 => "94–100%",
            _ => "Unknown"
        };

        var seeingText = entry.Seeing switch
        {
            1 => "Excellent",
            2 => "Good",
            3 => "Fair",
            4 => "Moderate",
            5 => "Poor",
            6 => "Very Poor",
            7 => "Bad",
            8 => "Terrible",
            _ => "Unknown"
        };

        var transparencyText = entry.Transparency switch
        {
            1 => "Excellent",
            2 => "Good",
            3 => "Fair",
            4 => "Moderate",
            5 => "Poor",
            6 => "Very Poor",
            7 => "Bad",
            8 => "Terrible",
            _ => "Unknown"
        };

        return $"{rating}\n☁️ Cloud cover: {cloudText}\n- Above Cloud Data:\n👁️ Seeing: {seeingText}\n🔭 Transparency: {transparencyText}";
    }
}