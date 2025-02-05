using System.Text.Json;

namespace GameBrain;

public abstract class LoadSavedGame
{
    public static GameSettings GetSavedGameSetting (string savedGameStateJson)
    {
        // Deserialize the JSON string to a dynamic object
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var gameData = JsonSerializer.Deserialize<JsonElement>(savedGameStateJson, options);

        // Extract the GameSettings values from the JSON object
        var gameSettingsJson = gameData.GetProperty("GameSettings");

        // Create a new GameSettings object with extracted values
        var gameConfig = new GameSettings
        {
            ConfigName = gameSettingsJson.GetProperty("ConfigName").GetString() ?? "Default",
            BoardSizeWidth = gameSettingsJson.GetProperty("BoardSizeWidth").GetInt32(),
            BoardSizeHeight = gameSettingsJson.GetProperty("BoardSizeHeight").GetInt32(),
            WinCondition = gameSettingsJson.GetProperty("WinCondition").GetInt32(),
            NumberOfPieces = gameSettingsJson.GetProperty("NumberOfPieces").GetInt32(),
            MoveGridAfterNMoves = gameSettingsJson.GetProperty("MoveGridAfterNMoves").GetInt32(),
            MovePieceAfterNMoves = gameSettingsJson.GetProperty("MovePieceAfterNMoves").GetInt32(),
            GridSizeWidth = gameSettingsJson.GetProperty("GridSizeWidth").GetInt32(),
            GridSizeHeight = gameSettingsJson.GetProperty("GridSizeHeight").GetInt32(),
            UsesGrid = gameSettingsJson.GetProperty("UsesGrid").GetBoolean()
        };

        return gameConfig;
    }

    public static GameState LoadGameState(string gameStateJson, GameSettings gameConfig)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var gameData = JsonSerializer.Deserialize<JsonElement>(gameStateJson, options);

        var gameBoardJson = gameData.GetProperty("GameBoard");
        var gameBoard = new EGamePiece[gameBoardJson.GetArrayLength()][];
        for (int i = 0; i < gameBoardJson.GetArrayLength(); i++)
        {
            var row = gameBoardJson[i];
            var boardRow = new EGamePiece[row.GetArrayLength()];
            for (int j = 0; j < row.GetArrayLength(); j++)
            {
                boardRow[j] = (EGamePiece)row[j].GetInt32();
            }
            gameBoard[i] = boardRow;
        }

        // Create the GameState object
        var gameState = new GameState(gameBoard, gameConfig)
        {
            GameBoard = gameBoard,
            PlayersSetting = (EGamePlayers)gameData.GetProperty("PlayersSetting").GetInt32(),
            PlayerX = (EGamePlayers)gameData.GetProperty("PlayerX").GetInt32(),
            PlayerO = (EGamePlayers)gameData.GetProperty("PlayerO").GetInt32(),
            PasswordX = gameData.GetProperty("PasswordX").GetString(),
            PasswordO = gameData.GetProperty("PasswordO").GetString(),
            NextMoveBy = (EGamePiece)gameData.GetProperty("NextMoveBy").GetInt32(),
            LastMoveBy = (EGamePiece)gameData.GetProperty("LastMoveBy").GetInt32(),
            Winner = (EGamePiece)gameData.GetProperty("Winner").GetInt32(),
            RemainingPiecesX = gameData.GetProperty("RemainingPiecesX").GetInt32(),
            RemainingPiecesO = gameData.GetProperty("RemainingPiecesO").GetInt32(),
            GridStartX = gameData.GetProperty("GridStartX").GetInt32(),
            GridStartY = gameData.GetProperty("GridStartY").GetInt32()
        };

        return gameState;
    }
}