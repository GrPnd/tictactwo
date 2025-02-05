using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryDb : IConfigRepository
{
    private readonly AppDbContext _context;

    public ConfigRepositoryDb(AppDbContext context)
    {
        _context = context;
        CheckAndCreateInitialConfig();
    }

    public List<string> GetConfigurationNames()
    {
        return _context.Configurations
            .OrderBy(config => config.ConfigName)
            .Select(config => config.ConfigName)
            .ToList();
    }

    public GameSettings GetConfigurationByName(string name)
    {
        var configuration = _context.Configurations.First(c => c.ConfigName == name);
        var gameSettings = ConfigToGameSettingsConverter.ConvertCtoGs(configuration);
        
        return gameSettings;
    }

    public void SaveConfiguration(GameSettings gameSettings)
    {
        var configuration = ConfigToGameSettingsConverter.ConvertGStoC(gameSettings);
        _context.Configurations.Add(configuration);
        _context.SaveChanges();
    }

    private void CheckAndCreateInitialConfig()
    {
        // If there are no configurations in the database, add initial hardcoded configurations
        if (!_context.Configurations.Any())
        {
            var hardcodedRepo = new ConfigRepositoryHardcoded();
            var optionNames = hardcodedRepo.GetConfigurationNames();

            foreach (var optionName in optionNames)
            {
                var gameOption = hardcodedRepo.GetConfigurationByName(optionName);
                var configuration = ConfigToGameSettingsConverter.ConvertGStoC(gameOption);
                
                _context.Configurations.Add(configuration);
            }

            _context.SaveChanges();
        }
    }


}