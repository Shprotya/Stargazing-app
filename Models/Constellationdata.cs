using Newtonsoft.Json;

namespace StargazingApp.Models;

/// <summary>
/// Helper class to deserialize the constellation JSON file
/// </summary>
public class ConstellationData
{
    [JsonProperty("constellations")]
    public List<ConstellationJson> Constellations { get; set; }
}

/// <summary>
/// JSON representation of constellation data (matches the JSON structure)
/// </summary>
public class ConstellationJson
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("abbreviation")]
    public string Abbreviation { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("bestVisibleMonth")]
    public string BestVisibleMonth { get; set; }

    [JsonProperty("hemisphere")]
    public string Hemisphere { get; set; }

    [JsonProperty("visibleLatitude")]
    public string VisibleLatitude { get; set; }

    [JsonProperty("brightestStar")]
    public string BrightestStar { get; set; }

    [JsonProperty("numberOfStars")]
    public int NumberOfStars { get; set; }

    [JsonProperty("area")]
    public int Area { get; set; }

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; }
}