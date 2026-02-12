using Newtonsoft.Json;

namespace StargazingApp.Models;

public class ApodData
{
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("explanation")]
    public string Explanation { get; set; } = string.Empty;

    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    [JsonProperty("media_type")]
    public string MediaType { get; set; } = string.Empty;

    [JsonProperty("copyright")]
    public string Copyright { get; set; } = string.Empty;
}
