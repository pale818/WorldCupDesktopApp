using System.Text.Json;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class SettingsService
{
    private const string FavoritePlayersPath = "favorites.json";
    private const string FavoriteTeamPath = "favorite_team.txt";
    private const string ConfigPath = "app_config.json";


    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    // Load favorite players from JSON file
    public List<Player> LoadFavoritePlayers()
    {
        if (!File.Exists(FavoritePlayersPath))
            return new List<Player>();

        var json = File.ReadAllText(FavoritePlayersPath);
        return JsonSerializer.Deserialize<List<Player>>(json, _jsonOptions) ?? new();
    }

    // Save favorite players to JSON file
    public void SaveFavoritePlayers(List<Player> players)
    {
        var json = JsonSerializer.Serialize(players, _jsonOptions);
        File.WriteAllText(FavoritePlayersPath, json);
    }

    // Load favorite team FIFA code
    public string LoadFavoriteTeam()
    {
        return File.Exists(FavoriteTeamPath)
            ? File.ReadAllText(FavoriteTeamPath)
            : "CRO"; // default fallback
    }

    // Save favorite team FIFA code
    public void SaveFavoriteTeam(string fifaCode)
    {
        File.WriteAllText(FavoriteTeamPath, fifaCode);
    }




    public AppConfig LoadAppConfig()
    {
        if (!File.Exists(ConfigPath))
            return new AppConfig(); // default if no file yet

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<AppConfig>(json, _jsonOptions) ?? new AppConfig();
    }

    public void SaveAppConfig(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        File.WriteAllText(ConfigPath, json);
    }

}
