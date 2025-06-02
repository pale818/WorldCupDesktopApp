using System.Text.Json.Serialization;

namespace WorldCup.Data.Models
{
    public class Weather
    {
        [JsonPropertyName("humidity")]
        public string Humidity { get; set; }

        [JsonPropertyName("temp_celsius")]
        public string TempCelsius { get; set; }

        [JsonPropertyName("temp_farenheit")]
        public string TempFarenheit { get; set; }

        [JsonPropertyName("wind_speed")]
        public string WindSpeed { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
