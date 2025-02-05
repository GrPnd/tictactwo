namespace GameBrain;

public class CheckWinLogic
{
    private readonly int _winCondition;
    private readonly EGamePiece[][] _board;
    private readonly int _dimX;
    private readonly int _dimY;
    private readonly int _dimXEnd;
    private readonly int _dimYEnd;
    private readonly TicTacTwoBrain _gameInstance;

    public CheckWinLogic(TicTacTwoBrain ticTacTwoBrain, int winCondition)
    {
        _winCondition = winCondition;
        _gameInstance = ticTacTwoBrain;
        _board = _gameInstance.GameBoard;
        
        if (ticTacTwoBrain.GameState.GameSettings.UsesGrid)
        {
            _dimX = _gameInstance.GameState.GridStartX;
            _dimY = _gameInstance.GameState.GridStartY;
            _dimXEnd = _dimX + _gameInstance.GameState.GameSettings.GridSizeWidth - 1;
            _dimYEnd = _dimY + _gameInstance.GameState.GameSettings.GridSizeHeight - 1;
        }
        else
        {
            _dimX = 0;
            _dimY = 0;
            _dimXEnd = _gameInstance.DimXBoard - 1;
            _dimYEnd = _gameInstance.DimYBoard - 1;
        }
    }
    
    public bool TryFindWinningMove(EGamePiece currentPiece, out int x, out int y)
    {
        for (x = _dimX; x <= _dimXEnd; x++)
        {
            for (y = _dimY; y <= _dimYEnd; y++)
            {
                if (_board[x][y] != EGamePiece.Empty)
                {
                    continue;
                }
                
                _board[x][y] = currentPiece;

                if (IsWin(true))
                {
                    _board[x][y] = EGamePiece.Empty;
                    return true;
                }
                
                _board[x][y] = EGamePiece.Empty;
            }
        }

        x = -1;
        y = -1;
        return false;
    }
    
    public bool IsDraw()
    {
        if (_gameInstance.GameState.RemainingPiecesO == 0 && _gameInstance.GameState.RemainingPiecesX == 0)
        {
            return true;
        }
        
        for (var y = _dimY; y <= _dimYEnd; y++)
        {
            for (var x = _dimX; x <= _dimXEnd; x++)
            {
                if (_board[x][y] == EGamePiece.Empty)
                {
                    return false; // Found an empty space, not a draw
                }
            }
        }
        return true; // No empty spaces found, it's a draw
    }
    
    public bool IsWin(bool checkingWin)
    {
        return CheckRows(checkingWin) || CheckColumns(checkingWin) || CheckDiagonals(checkingWin);
    }

    private bool CheckRows(bool checkingWin)
    {
        for (var y = _dimY; y <= _dimYEnd; y++)
        {
            for (var x = _dimX; x <= _dimXEnd - _winCondition + 1; x++)
            {
                // Check if current piece is not empty and matches the next pieces in the row
                if (_board[x][y] != EGamePiece.Empty &&
                    Enumerable.Range(0, _winCondition).All(offset => _board[x + offset][y] == _board[x][y]))
                {
                    if (!checkingWin)
                    {
                        _gameInstance.GameState.Winner = _board[x][y];
                        Console.WriteLine($"Player {_board[x][y]} wins!");
                    }
                    return true; // Found a winning condition in the row
                }
            }
        }
        return false; // No win found in any row
    }

    private bool CheckColumns(bool checkingWin)
    {
        for (var x = _dimX; x <= _dimXEnd; x++)
        {
            for (var y = _dimY; y <= _dimYEnd - _winCondition + 1; y++)
            {
                // Check if current piece is not empty and matches the next pieces in the column
                if (_board[x][y] != EGamePiece.Empty &&
                    Enumerable.Range(0, _winCondition).All(offset => _board[x] [y + offset] == _board[x][y]))
                {
                    if (!checkingWin)
                    {
                        _gameInstance.GameState.Winner = _board[x][y];
                        Console.WriteLine($"Player {_board[x][y]} wins!");
                    }
                    return true; // Found a winning condition in the column
                }
            }
        }
        return false; // No win found in any column
    }

    private bool CheckDiagonals(bool checkingWin)
    {
        // Check \ diagonal
        for (var x = _dimX; x <= _dimXEnd - _winCondition + 1; x++) // Adjust for diagonal wins
        {
            for (var y = _dimY; y <= _dimYEnd - _winCondition + 1; y++) // Iterate upward
            {
                // Check if current piece is not empty and matches the next pieces in the diagonal
                if (_board[x][y] != EGamePiece.Empty &&
                           Enumerable.Range(0, _winCondition).All(offset => _board[x + offset][y + offset] == _board[x][y]))
                {
                    if (!checkingWin)
                    {
                        _gameInstance.GameState.Winner = _board[x][y];
                        Console.WriteLine($"Player {_board[x][y]} wins!");
                    }
                    return true; // Found a winning condition in the \ diagonal
                }
            }
        }

        // Check / diagonal
        for (var x = _dimX; x <= _dimXEnd - _winCondition + 1; x++) // Adjust for diagonal wins
        {
            for (var y = _dimYEnd; y >= _dimY + _winCondition - 1; y--) // Iterate downward
            {
                // Check if current piece is not empty and matches the next pieces in the diagonal
                if (_board[x][y] != EGamePiece.Empty &&
                    Enumerable.Range(0, _winCondition).All(offset => _board[x + offset][y - offset] == _board[x][y]))
                {
                    if (!checkingWin)
                    {
                        _gameInstance.GameState.Winner = _board[x][y];
                        Console.WriteLine($"Player {_board[x][y]} wins!");
                    }
                    return true; // Found a winning condition in the / diagonal
                }
            }
        }
        return false; // No win found in any diagonal
    }
}