using System.Text.Json;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class ConfigService
{

    //used to "remeber" the last chosen gender,lang.....
    private const string ConfigPath = "settings.json"; // adjust path if needed

    public ConfigSettings Settings { get; private set; }

    public ConfigService()
    {
        if (File.Exists(ConfigPath))
        {
            var json = File.ReadAllText(ConfigPath);
            Settings = JsonSerializer.Deserialize<ConfigSettings>(json) ?? new ConfigSettings();
        }
        else
        {
            Settings = new ConfigSettings();
            Save(); // save default if missing
        }
    }


    //saves the current values in settings obj to json form
    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
