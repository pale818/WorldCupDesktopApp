using System.Text.Json;
using WorldCup.Data.Models;
using System.Windows;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;

namespace WorldCup.Data.Services;

public class MatchService
{

    //HttpClient: For making web requests
    //JsonSerializerOptions: Tells the JSON reader to ignore letter casing("team" vs "Team").
   //ConfigService: Provides access to the app settings for gender

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConfigService _config;

    //Builds the base URL for API calls depending on whether it’s men’s or women’s matches.
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


   

    // Get all matches
    // return is a List of Match, input parameter is gender and default is "man"
    //Fetches all matches, either from "men" of "women" file/API
    public async Task<List<Match>> GetAllMatchesAsync(string gender = "men")
    {
        //System.Diagnostics.Debug.WriteLine($"DataSource {_config.Settings.DataSource}");

        if (_config.Settings.DataSource == "local")
        {
            //if local, build a file path
            var path = $"./Data/{gender}/matches.json";
            System.Diagnostics.Debug.WriteLine("Path: " + path);

            if (File.Exists(path))
            {
                try
                {
                    /*
                     Loads file content as string,deserializes JSON into a List<Match>
                     If deserialization fails, returns empty list
                     */
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

        //if API is used:
        var url = $"{BaseUrl(gender)}/matches";
        
        try
        {

            var response = await _httpClient.GetStringAsync(url);
            System.Diagnostics.Debug.WriteLine("Response from API: " + response);
            return JsonSerializer.Deserialize<List<Match>>(response, _jsonOptions) ?? new();
        }
        catch (HttpRequestException httpEx)
        {
            //network issue
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve matches. Network issue or server is unreachable.");
}
        catch (JsonException jsonEx)
        {
            // bad or unexpected data
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

                    //for debugging:
                   // var newJsonFilterd = JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
                    //System.Diagnostics.Debug.WriteLine("newJsonFilterd: " + newJsonFilterd);

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
