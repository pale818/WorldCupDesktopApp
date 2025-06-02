using System.Text.Json.Serialization;

namespace WorldCup.Data.Models;

public class Player
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("captain")]
    public bool Captain { get; set; }

    [JsonPropertyName("shirt_number")]
    public int? ShirtNumber { get; set; }

    [JsonPropertyName("position")]

    public string Position { get; set; }
}
