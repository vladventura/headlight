using System.Text.Json.Serialization;

namespace Headlight.Models.JSON
{
    public class GameJSON
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("platform")]
        public string Platform { get; set; } = "";
        [JsonPropertyName("status")]
        public string Status { get; set; } = "";
    }
}
