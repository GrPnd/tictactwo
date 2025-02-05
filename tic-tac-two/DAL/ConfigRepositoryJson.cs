using GameBrain;

namespace DAL;

public class ConfigRepositoryJson : IConfigRepository
{
    public List<string> GetConfigurationNames()
    {
        CheckAndCreateInitialConfig();

        return Directory
            .GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension)
            .Select(file => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file)))
            .ToList();
    }

    public GameSettings? GetConfigurationByName(string name)
    {
        var configJsonStr = File.ReadAllText(FileHelper.BasePath + name + FileHelper.ConfigExtension);
        var config = System.Text.Json.JsonSerializer.Deserialize<GameSettings>(configJsonStr);
        return config;
    }

    public void SaveConfiguration(GameSettings gameSettings)
    {
        var optionJsonStr = System.Text.Json.JsonSerializer.Serialize(gameSettings);
        File.WriteAllText(FileHelper.BasePath + gameSettings.ConfigName + FileHelper.ConfigExtension, optionJsonStr);
    }


    private void CheckAndCreateInitialConfig()
    {
        if (!Directory.Exists(FileHelper.BasePath))
        { 
            Directory.CreateDirectory(FileHelper.BasePath);
        }

        var data = Directory.GetFiles(FileHelper.BasePath, "*" + FileHelper.ConfigExtension).ToList();
        if (data.Count == 0)
        {
            var hardcodedRepo = new ConfigRepositoryHardcoded();
            var optionNames = hardcodedRepo.GetConfigurationNames();
            foreach (var optionName in optionNames)
            {
                var gameOption = hardcodedRepo.GetConfigurationByName(optionName);
                var optionJsonStr = System.Text.Json.JsonSerializer.Serialize(gameOption);
                File.WriteAllText(FileHelper.BasePath + gameOption.ConfigName + FileHelper.ConfigExtension, optionJsonStr);
            }
        }
    }
}