using System.Text.Json;
using WorldCup.Data.Models;
using System.Windows;
using System.IO;

namespace WorldCup.Data.Services;

public class MatchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConfigService _config;

    private string BaseUrl(string gender) =>
    $"https://worldcup-vua.nullbit.hr/{gender.ToLower()}";

    public MatchService()
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }


    public MatchService(ConfigService configService)
    {
        _httpClient = new HttpClient();
        _config = configService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }


    //for logging into Visual Studio Output (debug)
    // System.Diagnostics.Debug

    // for logging / writing in the command terminal if app is made for command line
    // Console.WriteLine


    // Get all matches
    // return is a List of Match, input parameter is gender and default is "man"
    public async Task<List<Match>> GetAllMatchesAsync(string gender = "men")
    {
        System.Diagnostics.Debug.WriteLine($"DataSource {_config.Settings.DataSource}");

        if (_config.Settings.DataSource == "local")
        {
            var path = $"./Data/{gender}/matches.json";
            System.Diagnostics.Debug.WriteLine("Path: " + path);

            if (File.Exists(path))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(path);
                    return JsonSerializer.Deserialize<List<Match>>(json, _jsonOptions) ?? new();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Deserialization error (local): " + ex.Message);
                    return new(); // fallback to empty list
                }
            }
            return new(); // fallback if file not found
        }

        var url = $"{BaseUrl(gender)}/matches";
        
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            System.Diagnostics.Debug.WriteLine("Response from API: " + response);
            return JsonSerializer.Deserialize<List<Match>>(response, _jsonOptions) ?? new();
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve matches. Network issue or server is unreachable.");
}
        catch (JsonException jsonEx)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error: " + jsonEx.Message);
            throw new Exception("Failed to parse match data. The server response format is invalid.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error (API): " + ex.Message);
            throw new Exception("Failed to load matches. The response format is invalid.");
        }
    }



    // Get matches for a specific country
    public async Task<List<Match>> GetMatchesForTeamAsync(string gender, string fifaCode)
    {

        System.Diagnostics.Debug.WriteLine($"DataSource {_config.Settings.DataSource}");


        if (_config.Settings.DataSource == "local")
        {
            var path = $"./Data/{gender}/matches.json";
            System.Diagnostics.Debug.WriteLine("Path: " + path);

            if (File.Exists(path))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(path);
                    // returns all matches
                    var allMatches = JsonSerializer.Deserialize<List<Match>>(json, _jsonOptions) ?? new();

                    // return only for specific fifacode, all parts where this fifacode (team) is present
                    // in home team or away team
                    var filtered = allMatches
                    .Where(m =>
                        m.HomeTeam?.Code?.Equals(fifaCode, StringComparison.OrdinalIgnoreCase) == true ||
                        m.AwayTeam?.Code?.Equals(fifaCode, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();

                    var newJsonFilterd = JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
                    System.Diagnostics.Debug.WriteLine("newJsonFilterd: " + newJsonFilterd);

                    return filtered;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Deserialization error (local): " + ex.Message);
                    return new(); // fallback to empty list
                }
            }
            return new(); // fallback if file not found
        }

        var url = $"{BaseUrl(gender)}/matches/country?fifa_code={fifaCode}";

        try
        {

            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<Match>>(response, _jsonOptions) ?? new();
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve matches. Network issue or server is unreachable.");
        }
        catch (JsonException jsonEx)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error: " + jsonEx.Message);
            throw new Exception("Failed to parse match data. The server response format is invalid.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unexpected error: " + ex.Message);
            throw new Exception("An unexpected error occurred while loading matches.");
        }
    }


}
