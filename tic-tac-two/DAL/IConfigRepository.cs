using GameBrain;

namespace DAL;

public interface IConfigRepository
{
    public List<string> GetConfigurationNames();
    public GameSettings GetConfigurationByName(string name);
    public void SaveConfiguration(GameSettings gameSettings);
}