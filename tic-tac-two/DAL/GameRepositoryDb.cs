using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class GameRepositoryDb : IGameRepository
{
    
    private readonly AppDbContext _context;

    public GameRepositoryDb(AppDbContext context)
    {
        _context = context;
    }
    
    public string SaveGame(string jsonStateString, string configName)
    {
        Console.WriteLine("savedgame " + configName);
        var config = _context.Configurations.First(c => c.ConfigName == configName);
        var configSplit = configName.Split("-");
        var savedGame = new SaveGame()
        {
            GameName = configSplit[0] + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss"),
            State = jsonStateString,
            Configuration = config,
        };

        _context.SaveGames.Add(savedGame);
        _context.SaveChanges();
        return savedGame.GameName;
    }
    
    public void UpdateGame(string gameName, string newJsonState)
    {
        if (string.IsNullOrWhiteSpace(gameName))
        {
            throw new ArgumentException("GameName cannot be null or empty", nameof(gameName));
        }
        
        var gameToUpdate = _context.SaveGames.FirstOrDefault(g => g.GameName == gameName);

        if (gameToUpdate == null)
        {
            throw new KeyNotFoundException($"No game found with GameName: {gameName}");
        }
        
        gameToUpdate.State = newJsonState;
        _context.SaveChanges();
    }
    
    public string GetSavedGameStateByName(string gameName)
    {
        try
        {
            var savedGame = _context.SaveGames
                .Include(s => s.Configuration)
                .FirstOrDefault(g => g.GameName == gameName);

            if (savedGame == null)
            {
                return $"No game found with GameName: {gameName}";
            }

            return savedGame.State;
        }
        catch (Exception ex)
        {
            return $"An error occurred while searching for the saved game: {ex.Message}";
        }
    }
    
    public List<string> GetSavedGamesNames()
    {
        return _context.SaveGames
            .Select(s => s.GameName)
            .ToList();
    }
}