using System.Text.Json.Serialization;

namespace WorldCup.Data.Models
{
    public class Match
    {
        [JsonPropertyName("venue")]
        public string? Venue { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("fifa_id")]
        public string? FifaId { get; set; }

        [JsonPropertyName("weather")]
        public Weather Weather { get; set; }

        [JsonPropertyName("attendance")]
        public string Attendance { get; set; }

        [JsonPropertyName("officials")]
        public List<string> Officials { get; set; }

        [JsonPropertyName("stage_name")]
        public string StageName { get; set; }

        [JsonPropertyName("home_team_country")]
        public string HomeTeamCountry { get; set; }

        [JsonPropertyName("away_team_country")]
        public string AwayTeamCountry { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime Datetime { get; set; }

        [JsonPropertyName("winner")]
        public string Winner { get; set; }

        [JsonPropertyName("winner_code")]
        public string WinnerCode { get; set; }

        [JsonPropertyName("home_team")]
        public TeamScore HomeTeam { get; set; }

        [JsonPropertyName("away_team")]
        public TeamScore AwayTeam { get; set; }

        [JsonPropertyName("home_team_events")]
        public List<TeamEvent> HomeTeamEvents { get; set; }

        [JsonPropertyName("away_team_events")]
        public List<TeamEvent> AwayTeamEvents { get; set; }

        [JsonPropertyName("home_team_statistics")]
        public TeamStatistics HomeTeamStatistics { get; set; }

        [JsonPropertyName("away_team_statistics")]
        public TeamStatistics AwayTeamStatistics { get; set; }

        [JsonPropertyName("last_event_update_at")]
        public DateTime LastEventUpdateAt { get; set; }

        [JsonPropertyName("last_score_update_at")]
        public DateTime? LastScoreUpdateAt { get; set; }
    }
}
