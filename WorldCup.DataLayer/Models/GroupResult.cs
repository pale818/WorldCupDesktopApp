using System.Text.Json.Serialization;

namespace WorldCup.Data.Models;

public class GroupResult
{
    // JSON snake_case fields don't match C# PascalCase so this is needed
    // this is used to deserialize JSON properly
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("letter")]
    public string Letter { get; set; }
    [JsonPropertyName("ordered_teams")]
    public List<TeamResult> OrderedTeams { get; set; } = new(); // safe default
}

