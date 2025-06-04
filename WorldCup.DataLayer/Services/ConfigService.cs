using System.Text.Json;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class ConfigService
{
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

    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
