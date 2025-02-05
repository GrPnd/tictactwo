using DAL;
using Microsoft.EntityFrameworkCore;


namespace ConsoleApp;

public static class Program
{

    private static AppDbContext? _dbContext;
    private static IConfigRepository _configRepository = null!;
    private static IGameRepository _gameRepository = null!;

    // private static readonly IConfigRepository ConfigRepository = new ConfigRepositoryHardcoded();

    
    private const bool UsesDatabase = true;
    
    public static void Main()
    {
        InitializeRepositories();
        
        var menuController = new MenuController(_configRepository, _gameRepository);
        menuController.MainMenu();
    }

    private static void InitializeRepositories()
    {
        if (UsesDatabase)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={Path.Combine(FileHelper.BasePath, "app.db")}")
                .Options;

            _dbContext = new AppDbContext(options);
            _configRepository = new ConfigRepositoryDb(_dbContext);
            _gameRepository = new GameRepositoryDb(_dbContext);
        }
        else
        {
            _configRepository = new ConfigRepositoryJson();
            _gameRepository = new GameRepositoryJson();
        }
    }
}