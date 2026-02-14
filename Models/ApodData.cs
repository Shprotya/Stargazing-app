using Newtonsoft.Json;

namespace StargazingApp.Models;

/// <summary>
/// Data model representing NASA's Astronomy Picture of the Day (APOD).
/// This class maps to the JSON response from the NASA APOD API.
/// </summary>
public class ApodData
{
    // The title of the astronomical image or video.
    // Maps to the "title" field in the JSON response.
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    // The URL of the image or video.
    // For images, this is a direct link to the picture.
    // Maps to the "url" field in the JSON response.
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    // A detailed scientific explanation of the image or video.
    [JsonProperty("explanation")]
    public string Explanation { get; set; } = string.Empty;

    // The date this APOD was published in YYYY-MM-DD format.
    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    // Copyright information for the image or video.
    [JsonProperty("copyright")]
    public string Copyright { get; set; } = string.Empty;
}