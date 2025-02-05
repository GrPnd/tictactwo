using DAL;
using GameBrain;

namespace ConsoleApp;

public class ConsoleFunctions
{
    private readonly GameSettings _chosenConfig;
    private readonly TicTacTwoBrain _gameInstance;
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;
    
    public ConsoleFunctions(GameSettings chosenConfig, TicTacTwoBrain gameInstance, IGameRepository gameRepository, IConfigRepository configRepository)
    {
        _chosenConfig = chosenConfig;
        _gameInstance = gameInstance;
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }

    private void ParseCoordinates(string input, out int x, out int y)
    {
        x = -1; // Default to invalid coordinates
        y = -1; // Default to invalid coordinates

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input, please enter coordinates.");
            return;
        }

        var inputSplit = input.Split(",");
        if (inputSplit.Length != 2 || !int.TryParse(inputSplit[0].Trim(), out x) || !int.TryParse(inputSplit[1].Trim(), out y))
        {
            Console.WriteLine("Invalid coordinates format, please use 'x,y'.");
            return;
        }

        if (!(x >= 0 && x < _chosenConfig.BoardSizeWidth && y >= 0 && y < _chosenConfig.BoardSizeHeight))
        {
            Console.WriteLine("Move out of bounds, please try again.");
            x = -1;
            y = -1;
        }
    }

    public void ValidateAndMakeANewMove()
    {
        do
        {
            Console.WriteLine();
            Console.Write("Choose piece coordinates <x,y>: ");
            var input = Console.ReadLine()!;

            ParseCoordinates(input, out var inputX, out var inputY);

            if (inputX == -1 || inputY == -1)
            {
                Console.WriteLine("Invalid move, please try again.");
                continue;
            }

            if (_gameInstance.AnyPieceAlreadyExistInCoordinates(inputX, inputY))
            {
                Console.WriteLine("A piece already exists at this coordinate.");
                continue;
            }

            _gameInstance.MakeAMove(inputX, inputY, true);
            return;
            
        } while (true);
    }

    public void GetGridCoordinates()
    {
        do
        {
            Console.WriteLine("Choose grid coordinates <x,y>:");
            var input = Console.ReadLine()!;
            
            ParseCoordinates(input, out var gridXStart, out var gridYStart);

            if (gridXStart == -1 || gridYStart == -1)
            {
                Console.WriteLine("Invalid coordinates, please try again.");
                continue;
            }

            if (!_gameInstance.GridIsPlacedInBounds(gridXStart, gridYStart))
            {
                Console.WriteLine("Cannot place grid out of bounds.");
                continue;
            }

            _gameInstance.GameState.GridStartX = gridXStart;
            _gameInstance.GameState.GridStartY = gridYStart;
            return;
            
        } while (true);
    }
    
    public void HandleExistingPieceMovement(EGamePiece piece)
    {
        do
        {
            Console.Write("Write coordinates of an existing piece which you want to move <x,y>: ");
            var input = Console.ReadLine()!;

            ParseCoordinates(input, out var existingX, out var existingY);

            if (existingX == -1 || existingY == -1)
            {
                Console.WriteLine("Invalid coordinates, please try again.");
                continue;
            }

            if (!_gameInstance.PlayerPieceExistInCoordinates(existingX, existingY, piece))
            {
                Console.WriteLine("Piece doesn't exist in those coordinates or is not your piece. Try again.");
                continue;
            }

            GetNewPieceCoordinates(existingX, existingY);
            return;
            
        } while (true);
    }
    
    private void GetNewPieceCoordinates(int existingX, int existingY)
    {
        do
        {
            Console.WriteLine();
            Console.Write("Write new piece coordinates <x,y>: ");
            var input = Console.ReadLine()!;

            ParseCoordinates(input, out var newX, out var newY);

            if (newX == -1 || newY == -1)
            {
                Console.WriteLine("Invalid coordinates, please try again.");
                continue;
            }

            if (_gameInstance.AnyPieceAlreadyExistInCoordinates(newX, newY))
            {
                Console.WriteLine("A piece already exists at this coordinate.");
                continue; // Ask for new coordinates again
            }

            _gameInstance.RemovePiece(existingX, existingY);
            _gameInstance.MakeAMove(newX, newY, false);
            return;
            
        } while (true);
    }
    
    public void AskPasswordAndSave()
    {
        Console.WriteLine();
        if (_gameInstance.GameState.PlayersSetting == EGamePlayers.PvP)
        {
            Console.WriteLine("Enter a password for player X: ");
            ValidatePasswordInput(out var passwordX);
                
            Console.WriteLine("Enter a password for player O: ");
            ValidatePasswordInput(out var passwordO);
                
            _gameInstance.GameState.PasswordX = passwordX;
            _gameInstance.GameState.PasswordO = passwordO;
            _gameRepository.SaveGame(_gameInstance.GetGameStateJson(), _chosenConfig.ConfigName);  
        }
        else if (_gameInstance.GameState.PlayersSetting == EGamePlayers.PvA)
        {
            if (_gameInstance.GameState.PlayerX == EGamePlayers.Player)
            {
                Console.WriteLine("Enter a password for player X: ");
                ValidatePasswordInput(out var passwordX);
                    
                _gameInstance.GameState.PasswordX = passwordX;
                _gameRepository.SaveGame(_gameInstance.GetGameStateJson(), _chosenConfig.ConfigName);   
            }
            else if (_gameInstance.GameState.PlayerO == EGamePlayers.Player)
            {
                Console.WriteLine("Enter a password for player O: ");
                ValidatePasswordInput(out var passwordO);
                    
                _gameInstance.GameState.PasswordO = passwordO;
                _gameRepository.SaveGame(_gameInstance.GetGameStateJson(), _chosenConfig.ConfigName);   
            }
        }
        _configRepository.SaveConfiguration(_gameInstance.GameState.GameSettings);
    }
    
    private static void ValidatePasswordInput(out string? password)
    {
        password = Console.ReadLine();
        while (string.IsNullOrEmpty(password))
        {
            Console.WriteLine();
            Console.WriteLine("Password cannot be empty!");
            password = Console.ReadLine();
        }
    }
}