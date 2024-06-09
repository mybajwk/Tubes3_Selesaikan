namespace Selesaikan.Config;

using Newtonsoft.Json;
using System.IO;
using Selesaikan.Models;

public class Config
{
    public static void SaveConfiguration(AppConfig config, string filePath)
    {
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
    
    public static AppConfig LoadConfiguration(string filePath)
    {
        string json = File.ReadAllText(filePath);
        Console.WriteLine(Directory.GetCurrentDirectory());
        return JsonConvert.DeserializeObject<AppConfig>(json);
    }
}