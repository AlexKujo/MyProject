using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace RapidHelper.Apps.TextTemplates.Classes
{
    class JsonLoader
    {
        public static ObservableCollection<T> Load<T>(string jsonFilePath, string jsonName)
        {
            string jsonPath = Path.Combine(jsonFilePath, jsonName + ".json");

            string jsonString = File.ReadAllText(jsonPath);

            return JsonSerializer.Deserialize<ObservableCollection<T>>(jsonString);
        }
    }
}
