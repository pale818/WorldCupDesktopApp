using System.Text.Json.Serialization;

namespace WorldCup.Data.Models;

public class Team
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string? AlternateName { get; set; }

    [JsonPropertyName("fifa_code")]
    public string FifaCode { get; set; }

    [JsonPropertyName("group_id")]

    public int GroupId { get; set; }
    public string GroupLetter { get; set; }
}
