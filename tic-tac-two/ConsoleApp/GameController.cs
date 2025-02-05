using ConsoleUI;
using DAL;
using GameBrain;

namespace ConsoleApp;

public class GameController
{
    private static TicTacTwoBrain _gameInstance = null!;
    private readonly ConsoleFunctions _consoleFunctions;
    
    public GameController(GameSettings chosenConfig, IGameRepository gameRepository, IConfigRepository configRepository, TicTacTwoBrain gameInstance)
    {
        _gameInstance = gameInstance;
        
        _consoleFunctions = new ConsoleFunctions(chosenConfig, gameInstance, gameRepository, configRepository);
    }

    public string MainLoop()
    {
        PlayGame();
        return "mainLoop";
    }
    
    private void PlayGame()
    {
        do
        {
            Console.WriteLine();
            Visualizer.DrawBoard(_gameInstance);
            
            var gameStatus = StatusHandler();

            if (gameStatus is "Win" or "Draw")
            {
                break;
            }

            var currentPlayer = _gameInstance.GameState.NextMoveBy;
            
            Console.WriteLine();
            Console.WriteLine($"Current Player: {currentPlayer}");
            Console.WriteLine($"X - {_gameInstance.GameState.RemainingPiecesX} pieces remaining");
            Console.WriteLine($"O - {_gameInstance.GameState.RemainingPiecesO} pieces remaining");
            Console.WriteLine();
            

            if (currentPlayer == EGamePiece.X && _gameInstance.GameState.PlayerX == EGamePlayers.Ai)
            {
                var aiBrain = new AiBrain(EGamePiece.X, EGamePiece.O, _gameInstance);
                if (_gameInstance.PlayerCanPlaceNewPiece(EGamePiece.X))
                {
                    aiBrain.MakeMove();
                    continue;                    
                }
                _gameInstance.ChangeTurn();
                continue;
            }
            
            if (currentPlayer == EGamePiece.O && _gameInstance.GameState.PlayerO == EGamePlayers.Ai)
            {
                var aiBrain = new AiBrain(EGamePiece.O, EGamePiece.X, _gameInstance);
                if (_gameInstance.PlayerCanPlaceNewPiece(EGamePiece.O))
                {
                    aiBrain.MakeMove();
                    continue;                    
                }
                _gameInstance.ChangeTurn();
                continue;
            }
            
            MenuController.DisplayMovesOptions(_gameInstance, _consoleFunctions);
            
        } while (true);
    }
    

    private string StatusHandler()
    {
        var checkWinLogic = new CheckWinLogic(_gameInstance, _gameInstance.GameState.GameSettings.WinCondition);
        
        if (checkWinLogic.IsWin(false))
        {
            Console.WriteLine();
            OfferResetOrExit();
            return "Win";
        }
        if (checkWinLogic.IsDraw())
        {
            Console.WriteLine();
            Console.WriteLine("It's a draw!");
            OfferResetOrExit();
            return "Draw";
        }
        return "Game goes on";
    }


    private void OfferResetOrExit()
    {
        Console.WriteLine("Would you like to reset the game or return to the main menu?");
        Console.WriteLine("Press 'R' to Reset, or any other key to return to the main menu.");
        var choice = Console.ReadLine()?.Trim().ToUpper();

        if (choice == "R")
        {
            _gameInstance.ResetGame();
            if (_gameInstance.GameSettings.UsesGrid)
            {
                _consoleFunctions.GetGridCoordinates();
            }
            PlayGame();
        }
        else
        {
            Console.WriteLine("Returning to the main menu...");
        }
    }
}