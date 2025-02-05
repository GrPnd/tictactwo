namespace GameBrain;

public class TicTacTwoBrain
{
    public GameState GameState;
    public readonly GameSettings GameSettings;

    public TicTacTwoBrain(GameSettings gameSettings, GameState gameState)
    {
        //var gameBoard = CreateEmptyBoard(gameSettings.BoardSizeWidth, gameSettings.BoardSizeHeight);

        GameState = gameState;
        GameSettings = gameSettings;
        GameState.GameSettings = GameSettings;
    }

    public TicTacTwoBrain(GameSettings gameSettings)
    {
        var gameBoard = CreateEmptyBoard(gameSettings.BoardSizeWidth, gameSettings.BoardSizeHeight);

        GameState = new GameState(gameBoard, gameSettings);
        GameSettings = gameSettings;
        GameState.GameSettings = GameSettings;
    }

    public string GetGameStateJson() => GameState.ToString();
    
    public void SetGameStateJson(string gameState)
    {
        GameState = System.Text.Json.JsonSerializer.Deserialize<GameState>(gameState)!;
    }

    public int DimXBoard => GameState.GameBoard.Length;
    
    public int DimYBoard => GameState.GameBoard[0].Length;
    
    public EGamePiece GetNextMoveBy() => GameState.NextMoveBy;
    
    public EGamePiece[][] GameBoard => GetBoard();
    
    private EGamePiece[][] GetBoard()
    {
        var copyOfBoard = new EGamePiece[GameState.GameBoard.GetLength(0)][];
        for (var x = 0; x < GameState.GameBoard.Length; x++)
        {
            copyOfBoard[x] = new EGamePiece[GameState.GameBoard[x].Length];
            for (var y = 0; y < GameState.GameBoard[x].Length; y++)
            {
                copyOfBoard[x][y] = GameState.GameBoard[x][y];
            }
        }

        return copyOfBoard;
    }
    
    public void AddSettings(EGamePlayers selectedGameSetting, EGamePiece selectedPlayerSymbol, string? passwordX, string? passwordO)
    {
        GameState.PasswordX = passwordX;
        GameState.PasswordO = passwordO;
        
        GameState.PlayersSetting = selectedGameSetting;
        if (selectedGameSetting == EGamePlayers.PvP)
        {
            GameState.PlayerX = EGamePlayers.Player;
            GameState.PlayerO = EGamePlayers.Player;
        }
        else if (selectedGameSetting == EGamePlayers.PvA)
        {
            if (selectedPlayerSymbol == EGamePiece.X)
            {
                GameState.PlayerX = EGamePlayers.Player;
                GameState.PlayerO = EGamePlayers.Ai;
            }
            else
            {
                GameState.PlayerX = EGamePlayers.Ai;
                GameState.PlayerO = EGamePlayers.Player;
            }
        }
        else
        {
            GameState.PlayerX = EGamePlayers.Ai;
            GameState.PlayerO = EGamePlayers.Ai;
        }
    }

    public bool PasswordMatchesPlayer(string password, EGamePiece selectedPlayer)
    {
        if (selectedPlayer == EGamePiece.X && GameState.PasswordX == password)
        {
            return true;
        }
        
        if (selectedPlayer == EGamePiece.O && GameState.PasswordO == password)
        {
            return true;
        }
  
        return false;
        
    }
    
    public bool ShouldMovePiece(EGamePiece player)
    {
        var remainingMoves = player == EGamePiece.X ? GameState.RemainingPiecesX : GameState.RemainingPiecesO;
        return GameSettings.MovePieceAfterNMoves <= GameSettings.NumberOfPieces - remainingMoves;
    }

    public bool ShouldMoveGrid(EGamePiece player)
    {
        var remainingMoves = player == EGamePiece.X ? GameState.RemainingPiecesX : GameState.RemainingPiecesO;
        return GameSettings.MoveGridAfterNMoves <= GameSettings.NumberOfPieces - remainingMoves;
    }

    public bool IsCellInGrid(int x, int y)
    {
        if (!GameState.GameSettings.UsesGrid)
        {
            return false;
        }
        
        var gridStartX = GameState.GridStartX;
        var gridStartY = GameState.GridStartY;
        
        var gridEndX = gridStartX + GameState.GameSettings.GridSizeWidth - 1;
        var gridEndY = gridStartY + GameState.GameSettings.GridSizeHeight - 1;
        
        return x >= gridStartX && x <= gridEndX && y >= gridStartY && y <= gridEndY;
    }
    
    public bool MoveGrid(string direction)
    {
        if (string.IsNullOrEmpty(direction)) return false;

        // Check if the grid will still be within bounds after the move
        int newGridXStart = GameState.GridStartX;
        int newGridYStart = GameState.GridStartY;
        
        switch (direction)
        {
            case "Up":
                newGridYStart--;
                break;

            case "Down":
                newGridYStart++;
                break;

            case "Left":
                newGridXStart--;
                break;

            case "Right":
                newGridXStart++;
                break;

            case "UpLeft":
                newGridXStart--;
                newGridYStart--;
                break;

            case "UpRight":
                newGridXStart++;
                newGridYStart--;
                break;

            case "DownLeft":
                newGridXStart--;
                newGridYStart++;
                break;

            case "DownRight":
                newGridXStart++;
                newGridYStart++;
                break;

            default:
                return false; // Invalid direction
        }

        // Check if the new position would place the grid out of bounds
        if (GridIsPlacedInBounds(newGridXStart, newGridYStart))
        {
            // Apply the move if the grid remains in bounds
            GameState.GridStartX = newGridXStart;
            GameState.GridStartY = newGridYStart;
            return true; // Move was successful
        }

        // Return false to indicate the move was invalid
        return false;
        
    }
    
    public bool GridIsPlacedInBounds(int gridXStart, int gridYStart)
    {
        var gridXEnd = gridXStart + GameSettings.GridSizeWidth - 1;
        var gridYEnd = gridYStart + GameSettings.GridSizeHeight - 1;

        return gridXStart >= 0 && gridYStart >= 0 && gridXEnd < GameSettings.BoardSizeWidth && gridYEnd < GameSettings.BoardSizeHeight;
    }

    
    public bool AnyPieceAlreadyExistInCoordinates(int x, int y)
    {
        return GameState.GameBoard[x][y] != EGamePiece.Empty;
    }

    public bool PlayerPieceExistInCoordinates(int x, int y, EGamePiece piece)
    {
        return GameState.GameBoard[x][y] == piece;
    }

    public bool PlayerCanPlaceNewPiece(EGamePiece player)
    {
        if (player == EGamePiece.X && GameState.RemainingPiecesX > 0)
        {
            return true;
        }
        if (player == EGamePiece.O && GameState.RemainingPiecesO > 0)
        {
            return true;
        }

        return false;
    }
    

    public void MakeAMove(int x, int y, bool placedNewPiece)
    {
        if (GameState.GameBoard[x][y] != EGamePiece.Empty)
        {
            return;
        }
        
        GameState.GameBoard[x][y] = GameState.NextMoveBy;

        if (placedNewPiece)
        {
            UpdateRemainingPieces();
        }
        else
        {
            ChangeTurn();
        }
    }

    public void RemovePiece(int x, int y)
    {
        GameState.GameBoard[x][y] = EGamePiece.Empty;
    }
    
    private void UpdateRemainingPieces()
    {
        GameState.LastMoveBy = GameState.NextMoveBy;
        
        if (GameState.LastMoveBy == EGamePiece.X)
        {
            GameState.RemainingPiecesX--;
            GameState.NextMoveBy = EGamePiece.O;
        }
        else
        {
            GameState.RemainingPiecesO--;
            GameState.NextMoveBy = EGamePiece.X;
        }
    }

    public void ChangeTurn()
    {
        GameState.LastMoveBy = GameState.NextMoveBy;
        
        if (GameState.LastMoveBy == EGamePiece.X)
        {
            GameState.NextMoveBy = EGamePiece.O;
        }
        else
        {
            GameState.NextMoveBy = EGamePiece.X;
        }
    }
    
    public void ResetGame()
    {
        GameState.GameBoard = CreateEmptyBoard(GameState.GameSettings.BoardSizeWidth, GameState.GameSettings.BoardSizeHeight);
        GameState.NextMoveBy = EGamePiece.X;
        GameState.LastMoveBy = EGamePiece.O;
        GameState.RemainingPiecesX = GameState.GameSettings.NumberOfPieces;
        GameState.RemainingPiecesO = GameState.GameSettings.NumberOfPieces;
    }

    private static EGamePiece[][] CreateEmptyBoard(int boardSizeWidth, int boardSizeHeight)
    {
        var gameBoard = new EGamePiece[boardSizeWidth][];
        for (var x = 0; x < gameBoard.Length; x++)
        {
            gameBoard[x] = new EGamePiece[boardSizeHeight];
        }

        return gameBoard;
    }
}