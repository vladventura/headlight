using System.Text.Json.Serialization;

namespace Headlight.Models.JSON
{
    public class ImportJSON
    {
        [JsonPropertyName("games")]
        public List<GameJSON> Games { get; set; } = [];
        [JsonPropertyName("platforms")]
        public List<string> Platforms { get; set; } = [];
    }
}
