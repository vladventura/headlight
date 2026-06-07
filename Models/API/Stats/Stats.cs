using Headlight.Models.JSON;
using System.Text.Json.Serialization;

namespace Headlight.Models.API.Stats
{
    public class Stats
    {
        [JsonPropertyName("statsTimeFrames")]
        public List<StatsTimeFrame>? StatsTimeFrames{ get; set; } = [];
    }
}
