namespace GameBrain;

public class AiBrain
{
    private static readonly Random Random = new Random();
    private readonly EGamePiece _aiPiece;
    private readonly EGamePiece _opponentPiece;
    private TicTacTwoBrain _gameInstance;


    public AiBrain(EGamePiece aiPiece, EGamePiece opponentPiece, TicTacTwoBrain gameInstance)
    {
        _aiPiece = aiPiece;
        _opponentPiece = opponentPiece;
        _gameInstance = gameInstance;
    }

    private static bool TryFindBlockingMove(CheckWinLogic checkWinLogic, EGamePiece opponentPiece, out int x, out int y)
    {
        return checkWinLogic.TryFindWinningMove(opponentPiece, out x, out y);
    }


    public void MakeMove()
    {
        var checkWinLogic = new CheckWinLogic(_gameInstance, _gameInstance.GameState.GameSettings.WinCondition);
        if (checkWinLogic.TryFindWinningMove(_aiPiece, out var x, out var y))
        {
            _gameInstance.MakeAMove(x, y, true);
        }
        else if (TryFindBlockingMove(checkWinLogic, _opponentPiece, out x, out y))
        {
            _gameInstance.MakeAMove(x, y, true);
        }
        else
        {
            GenerateRandomCoordinates(out x, out y);
            _gameInstance.MakeAMove(x, y, true);
        }
    }

    private void GenerateRandomCoordinates(out int x, out int y)
    {
        int randomX;
        int randomY;
        var attempts = 0;
        const int maxAttempts = 100;

        do
        {
            // Generate random coordinates based on whether the grid is used
            if (_gameInstance.GameState.GameSettings.UsesGrid)
            {
                randomX = Random.Next(_gameInstance.GameState.GridStartX,
                    _gameInstance.GameState.GridStartX + _gameInstance.GameState.GameSettings.GridSizeWidth);
                randomY = Random.Next(_gameInstance.GameState.GridStartY,
                    _gameInstance.GameState.GridStartY + _gameInstance.GameState.GameSettings.GridSizeHeight);
            }
            else
            {
                randomX = Random.Next(0, _gameInstance.GameState.GameSettings.BoardSizeWidth);
                randomY = Random.Next(0, _gameInstance.GameState.GameSettings.BoardSizeHeight);
            }

            // Check if the generated coordinates are valid
            attempts++;

            if (attempts >= maxAttempts)
            {
                throw new InvalidOperationException("No valid moves left on the board.");
            }
        } while (_gameInstance.AnyPieceAlreadyExistInCoordinates(randomX, randomY));

        // Output the final valid coordinates
        x = randomX;
        y = randomY;
    }
}