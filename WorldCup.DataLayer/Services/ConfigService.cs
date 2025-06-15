using System.Text.Json;
using System.Threading.Channels;
using WorldCup.Data.Models;

namespace WorldCup.Data.Services;

public class ConfigService
{

    //Defines the file name where the settings are stored.
    private const string ConfigPath = "settings.json";

    //This holds the current settings in memory, so other parts of the app can read it (but not set it directly).
    public ConfigSettings Settings { get; private set; }

    public ConfigService()
    {
        if (File.Exists(ConfigPath))
        {
            // deserializes (loads) the JSON content into the Settings object.
            var json = File.ReadAllText(ConfigPath);
            Settings = JsonSerializer.Deserialize<ConfigSettings>(json) ?? new ConfigSettings();
        }
        else
        {
            //it creates a new default ConfigSettings object and saves it to disk.
            Settings = new ConfigSettings();
            Save(); 
        }
    }


    //Reloads the settings from file (if it exists).
    //this can be used to re-load settings later, e.g.
    //if the file is changed in another form (SettingsFrom in my case)
    public void LoadConfig()
    {
        if (File.Exists(ConfigPath))
        {
            var json = File.ReadAllText(ConfigPath);
            Settings = JsonSerializer.Deserialize<ConfigSettings>(json) ?? new ConfigSettings();
        }
    }

    //Converts the current Settings object to JSON and writes it to settings.json.
    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}
