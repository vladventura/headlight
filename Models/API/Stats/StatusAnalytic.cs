using System.Text.Json.Serialization;

namespace Headlight.Models.API.Stats
{
    public class StatusAnalytic
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";
        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }
        [JsonPropertyName("avgTimeToFinish")]
        public TimeSpan? AvgTimeToFinish { get; set; }
    }
}
