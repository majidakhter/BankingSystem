using Newtonsoft.Json;

public static class JsonSeedDataLoader
{
    public static List<T> LoadSeedData<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Warning: JSON seed data file not found at {filePath}");
            return new List<T>();
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(jsonContent);
            //return JsonSerializer.Deserialize<List<T>>(jsonContent); 
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON from {filePath}: {ex.Message}");
            return new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while reading {filePath}: {ex.Message}");
            return new List<T>();
        }
    }

    public static Dictionary<string, List<object>> LoadMultipleSeedData(string directoryPath, Dictionary<string, Type> fileToTypeMap)
    {
        var allSeedData = new Dictionary<string, List<object>>();

        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine($"Warning: Seed data directory not found at {directoryPath}");
            return allSeedData;
        }

        foreach (var entry in fileToTypeMap)
        {
            string fileName = entry.Key;
            Type dataType = entry.Value;
            string className = dataType.Name;
            string filePath = Path.Combine(directoryPath, fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Warning: JSON seed data file not found at {filePath}");
                continue;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    // Add other settings as needed, e.g., MissingMemberHandling, Converters, etc.
                };
                List<object> data = JsonConvert.DeserializeObject<List<object>>(jsonContent, settings);
                if (data != null)
                {
                    allSeedData.Add(Path.GetFileNameWithoutExtension(fileName), data);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON from {filePath}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading {filePath}: {ex.Message}");
            }
        }
        return allSeedData;
    }
}