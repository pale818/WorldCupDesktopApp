using System.Text.Json;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class TeamService
{
    private readonly HttpClient _httpClient;
    private readonly ConfigService _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public TeamService(ConfigService configService)
    {
        _httpClient = new HttpClient();
        _config = configService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string BaseUrl(string gender) =>
        $"https://worldcup-vua.nullbit.hr/{gender.ToLower()}";


    /*
       Deserialize is doing:
       converts JSON like:
        [
            {"country": "Croatia", "fifa_code": "CRO"},
            {"country": "Brazil", "fifa_code": "BRA"}
        ]

    into object like:

        teams[0].Country = "Croatia";
        teams[0].FifaCode = "CRO";

        teams[1].Country = "Brazil";
        teams[1].FifaCode = "BRA";
     */
    //Console.WriteLine($"GetTeamsAsync: JsonSerializer {JsonSerializer.Deserialize<List<Team>>(response, _jsonOptions)}");
    public async Task<List<Team>> GetTeamsAsync(string gender = "men")
    {
        try
        {
            if (_config.Settings.DataSource == "local")
            {
                var file = $"./Data/{gender}_teams.json";
                System.Diagnostics.Debug.WriteLine($"[GetTeamsAsync] Local file: {file}");

                if (File.Exists(file))
                {
                    var json = await File.ReadAllTextAsync(file);
                    return JsonSerializer.Deserialize<List<Team>>(json, _jsonOptions) ?? new();
                }

                System.Diagnostics.Debug.WriteLine("[GetTeamsAsync] Local file not found.");
                return new(); // empty list fallback
            }

            var url = $"{BaseUrl(gender)}/teams";
            System.Diagnostics.Debug.WriteLine($"[GetTeamsAsync] API URL: {url}");

            var response = await _httpClient.GetStringAsync(url);
            System.Diagnostics.Debug.WriteLine($"response: { response}");

            return JsonSerializer.Deserialize<List<Team>>(response, _jsonOptions) ?? new();
        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve teams. Network issue or server is unreachable.");
        }
        catch (JsonException jsonEx)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error: " + jsonEx.Message);
            throw new Exception("Failed to parse team data. Server response format may be incorrect.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unexpected error: " + ex.Message);
            throw new Exception("An unexpected error occurred while loading teams.");
        }
    }


    public async Task<List<TeamResult>> GetTeamResultsAsync(string gender = "men")
    {
        try
        {
            var url = $"{BaseUrl(gender)}/teams/results";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<TeamResult>>(response, _jsonOptions) ?? new();

        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve teams. Network issue or server is unreachable.");
        }
        catch (JsonException jsonEx)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error: " + jsonEx.Message);
            throw new Exception("Failed to parse team data. Server response format may be incorrect.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unexpected error: " + ex.Message);
            throw new Exception("An unexpected error occurred while loading teams.");
        }
    }

    public async Task<List<GroupResult>> GetGroupResultsAsync(string gender = "men")
    {

        try
        {
            var url = $"{BaseUrl(gender)}/teams/group_results";
            //System.Diagnostics.Debug.WriteLine($"url {url}");
            var response = await _httpClient.GetStringAsync(url);
            // System.Diagnostics.Debug.WriteLine($"response {response}");
            return JsonSerializer.Deserialize<List<GroupResult>>(response, _jsonOptions) ?? new();

        }
        catch (HttpRequestException httpEx)
        {
            System.Diagnostics.Debug.WriteLine("HTTP request error: " + httpEx.Message);
            throw new Exception("Failed to retrieve teams. Network issue or server is unreachable.");
        }
        catch (JsonException jsonEx)
        {
            System.Diagnostics.Debug.WriteLine("Deserialization error: " + jsonEx.Message);
            throw new Exception("Failed to parse team data. Server response format may be incorrect.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Unexpected error: " + ex.Message);
            throw new Exception("An unexpected error occurred while loading teams.");
        }
    }
}
