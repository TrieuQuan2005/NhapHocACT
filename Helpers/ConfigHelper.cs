using Newtonsoft.Json;

public static class ConfigHelper
{
    public static T LoadFeatureConfig<T>(string featureName)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Configs", "Features", $"{featureName}.json");
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }
}