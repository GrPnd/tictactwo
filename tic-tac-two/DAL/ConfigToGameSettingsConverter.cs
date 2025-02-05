using Domain;
using GameBrain;

namespace DAL;

public static class ConfigToGameSettingsConverter
{
    public static GameSettings ConvertCtoGs(Configuration configuration)
    {
        var gameSettings = new GameSettings
        {
            ConfigName = configuration.ConfigName,
            BoardSizeWidth = configuration.BoardSizeWidth,
            BoardSizeHeight = configuration.BoardSizeHeight,
            WinCondition = configuration.WinCondition,
            NumberOfPieces = configuration.NumberOfPieces,
            MoveGridAfterNMoves = configuration.MoveGridAfterNMoves,
            MovePieceAfterNMoves = configuration.MovePieceAfterNMoves,
            GridSizeWidth = configuration.GridSizeWidth,
            GridSizeHeight = configuration.GridSizeHeight,
            UsesGrid = configuration.UsesGrid
        };
        return gameSettings;
    }
    
    public static Configuration ConvertGStoC(GameSettings gameSettings)
    {
        var configuration = new Configuration
        {
            ConfigName = gameSettings.ConfigName,
            BoardSizeWidth = gameSettings.BoardSizeWidth,
            BoardSizeHeight = gameSettings.BoardSizeHeight,
            WinCondition = gameSettings.WinCondition,
            NumberOfPieces = gameSettings.NumberOfPieces,
            MoveGridAfterNMoves = gameSettings.MoveGridAfterNMoves,
            MovePieceAfterNMoves = gameSettings.MovePieceAfterNMoves,
            GridSizeWidth = gameSettings.GridSizeWidth,
            GridSizeHeight = gameSettings.GridSizeHeight,
            UsesGrid = gameSettings.UsesGrid
        };
        return configuration;
    }
}