using System.Text.Json.Serialization;

namespace Headlight.Models.API.Stats
{
    public enum TimeFrameKeys
    {
        Month,
        Year,
        AllTime
    }

    public class StatsTimeFrame
    {
        [JsonPropertyName("statusAnalytics")]
        public List<StatusAnalytic> StatusAnalytics { get; set; } = [];
        [JsonPropertyName("timeFrameKey")]
        public TimeFrameKeys TimeFrameKey { get; set; }
        public string TimeFrameName { get; set; } = "";
    }
}
