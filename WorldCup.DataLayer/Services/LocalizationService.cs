﻿using System;
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

            //path  based on if en or hr is choosen
            var path = $"./Lang/lang_{langCode}.json";
            if (File.Exists(path))
            {
                //read from file and saved to var json
                var json = File.ReadAllText(path);
                //transforms from json file to dictionary like teams[0].Country = "Croatia";
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                _translations = new(); // Fallback
            }
            //System.Diagnostics.Debug.WriteLine("Path: " + path);
            //System.Diagnostics.Debug.WriteLine(("Exists: " + File.Exists(path)));
        }

        //If _translations is not null AND it contains the given key,return the value for that key.Otherwise, just return
        //the key itself.
        // If _translations has "favouriteTeam" key → return its translation value
        // If not → return "favouriteKey"
        public string this[string key] => _translations?.ContainsKey(key) == true ? _translations[key] : key;
    }
}