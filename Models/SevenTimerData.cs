using Newtonsoft.Json;

namespace StargazingApp.Models;

public class SevenTimerResponse
{
    [JsonProperty("dataseries")]
    public List<SevenTimerEntry> DataSeries { get; set; } = new();
}

public class SevenTimerEntry
{
    [JsonProperty("timepoint")] public int Timepoint { get; set; }    // hours from now
    [JsonProperty("cloudcover")] public int CloudCover { get; set; }   // 1-9 (1=clear, 9=overcast)
    [JsonProperty("seeing")] public int Seeing { get; set; }       // 1-8 (1=best, 8=worst)
    [JsonProperty("transparency")] public int Transparency { get; set; } // 1-8 (1=best, 8=worst)
}