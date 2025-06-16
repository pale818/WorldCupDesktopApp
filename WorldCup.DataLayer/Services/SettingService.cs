using System.Text.Json;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class SettingsService
{
    private const string FavoritePlayersPath = "favorites.json";
    //private const string FavoriteTeamPath = "favorite_team.txt";
    //private const string ConfigPath = "app_config.json";



    /*
     This sets up JSON options:
    WriteIndented = true: easy to read
    PropertyNameCaseInsensitive = true: Allows matching JSON even if key casing differs.
     */
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    // Load favorite players from JSON file, reads and deserializes the content into a List<Player>
    public List<Player> LoadFavoritePlayers()
    {
        if (!File.Exists(FavoritePlayersPath))
            return new List<Player>();

        var json = File.ReadAllText(FavoritePlayersPath);
        return JsonSerializer.Deserialize<List<Player>>(json, _jsonOptions) ?? new();
    }

    // Save favorite players to JSON file(favourite.json)
    public void SaveFavoritePlayers(List<Player> players)
    {
        var json = JsonSerializer.Serialize(players, _jsonOptions);
        File.WriteAllText(FavoritePlayersPath, json);
    }


    //Reads the app's configuration from app_config.json
    /*public AppConfig LoadAppConfig()
    {
        if (!File.Exists(ConfigPath))
            return new AppConfig(); // default if no file yet

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? new AppConfig();
    }


    //Serializes the AppConfig object and saves it to app_config.json
    public void SaveAppConfig(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        File.WriteAllText(ConfigPath, json);
    }
    */
}
