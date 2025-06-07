using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorldCup.Data.Services
{
    public class LocalizationService
    {
        private Dictionary<string, string> _translations;

        public void LoadLanguage(string langCode)
        {

            var path = $"./Lang/lang_{langCode}.json";
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                _translations = new(); // Fallback
            }
            //System.Diagnostics.Debug.WriteLine("Path: " + path);
            //System.Diagnostics.Debug.WriteLine(("Exists: " + File.Exists(path)));
        }

        public string this[string key] => _translations?.ContainsKey(key) == true ? _translations[key] : key;
    }
}