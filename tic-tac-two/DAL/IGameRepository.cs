namespace DAL;

public interface IGameRepository
{
    public string SaveGame(string jsonStateString, string configName);
    public void UpdateGame(string gameName, string newJsonState);
    public string GetSavedGameStateByName(string gameName);
    public List<string> GetSavedGamesNames();
}