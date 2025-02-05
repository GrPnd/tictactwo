
using Domain;
using GameBrain;

namespace DAL;

public class ConfigRepositoryHardcoded : IConfigRepository
{
    private readonly List<GameSettings> _gameConfigurations =
    [
        new GameSettings()
        {
            ConfigName = "Classical Tic-Tac-Toe"
        },

        new GameSettings()
        {
            ConfigName = "Regular Tic-Tac-Two",
            BoardSizeWidth = 5,
            BoardSizeHeight = 5,
            WinCondition = 3,
            NumberOfPieces = 4,
            MoveGridAfterNMoves = 2,
            MovePieceAfterNMoves = 2,
            GridSizeWidth = 3,
            GridSizeHeight = 3,
            UsesGrid = true,
        },

        new GameSettings()
        {
            ConfigName = "Custom"
        }

    ];

    public List<string> GetConfigurationNames()
    {
        return _gameConfigurations
            .OrderBy(x => x.ConfigName)
            .Select(config => config.ConfigName)
            .ToList();
    }

    public GameSettings GetConfigurationByName(string name)
    {
        return _gameConfigurations.Single(c => c.ConfigName == name);
    }

    public void SaveConfiguration(GameSettings gameSettings)
    {
        throw new NotImplementedException();
    }

    public List<string> GetSavedGamesNames()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SaveGame> GetSavedGames()
    {
        throw new NotImplementedException();
    }

    public string GetSavedGamePasswordByName(string name)
    {
        throw new NotImplementedException();
    }
}