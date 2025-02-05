using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PlayGame : PageModel
{

    private readonly IGameRepository _gameRepository;

    public PlayGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }


    [BindProperty(SupportsGet = true)] public string GameName { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string Password { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string Message { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string ErrorMsg { get; set; } = default!;

    
    [BindProperty] public int X { get; set; }
    [BindProperty] public int Y { get; set; }
    [BindProperty] public int? SelectedX { get; set; }
    [BindProperty] public int? SelectedY { get; set; }

    [TempData] public bool MovedPiece { get; set; } = false;
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    
    public IActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(GameName))
        {
            TempData.Keep("MovedPiece");
            
            LoadGame(GameName);
            
            var status = CheckWinLogic();
            if (status == "Win")
            {
                Message = "Player " + TicTacTwoBrain.GameState.Winner + " wins!";
                return Page();
            }
            if (status == "Draw")
            {
                Message = "Draw!";
                return Page();
            }
            
            return HandleAi();
        }

        return Page();
    }

    public IActionResult HandleAi()
    {
        var currentPlayer = TicTacTwoBrain.GameState.NextMoveBy;
        if (currentPlayer == EGamePiece.X && TicTacTwoBrain.GameState.PlayerX == EGamePlayers.Ai)
        {
            var aiBrain = new AiBrain(EGamePiece.X, EGamePiece.O, TicTacTwoBrain);
            if (TicTacTwoBrain.PlayerCanPlaceNewPiece(EGamePiece.X))
            {
                aiBrain.MakeMove();
                _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());
                
                return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });               
            }
            
            TicTacTwoBrain.ChangeTurn();
            _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());
            
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password }); 
        }
            
        if (currentPlayer == EGamePiece.O && TicTacTwoBrain.GameState.PlayerO == EGamePlayers.Ai)
        {
            var aiBrain = new AiBrain(EGamePiece.O, EGamePiece.X, TicTacTwoBrain);
            if (TicTacTwoBrain.PlayerCanPlaceNewPiece(EGamePiece.O))
            {
                aiBrain.MakeMove();
                _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());

                return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });             
            }

            TicTacTwoBrain.ChangeTurn();
            _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());
            
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password }); 
        }
        return Page();
    }
    
    public IActionResult OnPost()
    {
        if (!string.IsNullOrEmpty(GameName))
        {
            LoadGame(GameName);
        }

        if (Request.Form["reset"] == "true")
        {
            TicTacTwoBrain.ResetGame();
            _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());

            if (TicTacTwoBrain.GameState.GameSettings.UsesGrid)
            {
                return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = Password });
            }
            
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });
        }
        
        
        if (!string.IsNullOrEmpty(Request.Form["GridDirection"]))
        {
            return MoveGrid(Request.Form["GridDirection"]!);
        }
        
        if (SelectedX.HasValue && SelectedY.HasValue)
        {
            return RemovePiece(SelectedX.Value, SelectedY.Value);
        }
        
        if (X >= 0 && Y >= 0)
        {
            return PlaceNewPiece(X, Y);
        }
        
        return Page();
    }

    public void LoadGame(string gameName)
    {
        var savedGameState = _gameRepository.GetSavedGameStateByName(gameName);
        
        if (savedGameState == null)
        {
            Message = "Failed to load the game state. The game data might be missing or corrupted.";
            return;
        }
        
        var gameSettings = LoadSavedGame.GetSavedGameSetting(savedGameState);
            
        TicTacTwoBrain = new TicTacTwoBrain(gameSettings);
        
        TicTacTwoBrain.SetGameStateJson(savedGameState);
    }

    public IActionResult PlaceNewPiece(int x, int y)
    {
        var movedPiece = TempData["MovedPiece"] != null && (bool)TempData["MovedPiece"];
        
        if (movedPiece)
        {
            TicTacTwoBrain.MakeAMove(x, y, false);
            TempData["MovedPiece"] = false;
        }
        else
        {
            TicTacTwoBrain.MakeAMove(x, y, true);    
        }
        
        _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());
        
        return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });
    }
    

    public IActionResult RemovePiece(int x, int y)
    {
        var movedPiece = TempData["MovedPiece"] != null && (bool)TempData["MovedPiece"];

        // Prevent removing a piece if a move has already been made
        if (movedPiece)
        {
            ErrorMsg = "Cannot place piece because a piece is already in these coordinates.";
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password, errorMsg = ErrorMsg });
        }
        
        TicTacTwoBrain.RemovePiece(x, y);
        
        _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());

        TempData["MovedPiece"] = true;

        return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });
    }
    
    
    public IActionResult MoveGrid(string direction)
    {
        var result = TicTacTwoBrain.MoveGrid(direction);
        if (result)
        {
            TicTacTwoBrain.ChangeTurn();
            _gameRepository.UpdateGame(GameName, TicTacTwoBrain!.GetGameStateJson());  
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });
        }
        
        ErrorMsg = "Cannot move grid out of bounds!";
        return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password, errorMsg = ErrorMsg });
        
        
    }
    
    
    private string CheckWinLogic()
    {
        var winLogic = new CheckWinLogic(TicTacTwoBrain, TicTacTwoBrain.GameState.GameSettings.WinCondition);
        
        if (winLogic.IsWin(false))
        {
            return "Win";
        }

        if (winLogic.IsDraw())
        {
            return "Draw";
        }
        return "Ongoing";
    }


    public bool GameNotOverAndIsPlayersTurn()
    {
        return string.IsNullOrEmpty(Message) && 
               TicTacTwoBrain.PasswordMatchesPlayer(Password, TicTacTwoBrain.GameState.NextMoveBy);
    }

    public bool CanPlayerMovePiece(int x, int y)
    {
        return TicTacTwoBrain.GetNextMoveBy() == TicTacTwoBrain.GameBoard[x][y] && 
               TicTacTwoBrain.ShouldMovePiece(TicTacTwoBrain.GameState.NextMoveBy);
    }

    public bool IsCellEmpty(int x, int y)
    { 
        return TicTacTwoBrain.GameBoard[x][y] == EGamePiece.Empty;
    }
    
    public IActionResult ResetGame()
    {
        Console.WriteLine("Aa");
        if (!string.IsNullOrEmpty(GameName))
        {
            LoadGame(GameName);
        }
        
        TicTacTwoBrain.ResetGame();
        _gameRepository.UpdateGame(GameName, TicTacTwoBrain.GetGameStateJson());


        return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });
    }
}