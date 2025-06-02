using System.Text.Json.Serialization;

namespace WorldCup.Data.Models
{
    public class TeamEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_of_event")]
        public string TypeOfEvent { get; set; }

        [JsonPropertyName("player")]
        public string Player { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
}

