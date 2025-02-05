using System.ComponentModel.DataAnnotations;
using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ConfigureCustomGame : PageModel
{
    private readonly IGameRepository _gameRepository;
    private readonly IConfigRepository _configRepository;

    public ConfigureCustomGame(IGameRepository gameRepository, IConfigRepository configRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }
    
    [BindProperty(SupportsGet = true)] public EGamePlayers GameSetting { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public EGamePiece Player { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string PasswordX { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string PasswordO { get; set; } = default!;



    [BindProperty, Required, Range(2, 20, ErrorMessage = "Board width must be between 2 and 20.")]
    public int BoardSizeWidth { get; set; } = 2;

    [BindProperty, Required, Range(2, 20, ErrorMessage = "Board height must be between 2 and 20.")]
    public int BoardSizeHeight { get; set; } = 2;

    [BindProperty, Required, Range(2, 10, ErrorMessage = "Win condition must be between 2 and 10.")]
    public int WinCondition { get; set; } = 2;

    [BindProperty, Required, Range(2, 100, ErrorMessage = "Number of pieces must be between 2 and 100.")]
    public int NumberOfPieces { get; set; } = 2;

    [BindProperty] public int MovePieceAfterNMoves { get; set; }

    [BindProperty] public bool UsesGrid { get; set; } = false;

    [BindProperty] public int MoveGridAfterNMoves { get; set; }

    [BindProperty] public int GridSizeWidth { get; set; }

    [BindProperty] public int GridSizeHeight { get; set; }
    
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    public string GameName { get; set; } = default!;
    public List<string> Errors { get; set; } = new List<string>();
    
    
    public IActionResult OnPost()
    {
        var errors = new List<string>();

        var maxBoardValue = UsesGrid
            ? Math.Min(GridSizeWidth, GridSizeHeight)
            : Math.Min(BoardSizeWidth, BoardSizeHeight);

        var maxWinCondition = Math.Min(maxBoardValue, NumberOfPieces);
        const int minValue = 1;

        if (WinCondition < minValue || WinCondition > maxWinCondition)
        {
            errors.Add($"Win Condition must be between {minValue} and {maxWinCondition}.");
        }
        
        if (NumberOfPieces < WinCondition)
        {
            errors.Add($"WinCondition can't be bigger than {NumberOfPieces}.");
        }

        if (MovePieceAfterNMoves < 0)
        {
            errors.Add("Move piece after n moves cannot be negative!");
        }

        if (UsesGrid)
        {
            if (GridSizeWidth < 1 || GridSizeWidth > BoardSizeWidth)
            {
                errors.Add($"Grid width must be between 1 and {BoardSizeWidth}.");
            }

            if (GridSizeHeight < 1 || GridSizeHeight > BoardSizeHeight)
            {
                errors.Add($"Grid height must be between 1 and {BoardSizeHeight}.");
            }

            if (MoveGridAfterNMoves < 0)
            {
                errors.Add("Move grid after n moves cannot be negative!");
            }
        }

        if (errors.Any())
        {
            Errors = errors;
            return Page();
        }
        
        
        var gameSettings = new GameSettings
        {
            ConfigName = $"Custom-{DateTime.Now:yyyy-MM-dd-HH_mm_ss}",
            BoardSizeWidth = BoardSizeWidth,
            BoardSizeHeight = BoardSizeHeight,
            WinCondition = WinCondition,
            NumberOfPieces = NumberOfPieces,
            MovePieceAfterNMoves = MovePieceAfterNMoves,
            UsesGrid = UsesGrid,
            MoveGridAfterNMoves = MoveGridAfterNMoves,
            GridSizeWidth = GridSizeWidth,
            GridSizeHeight = GridSizeHeight
        };

        _configRepository.SaveConfiguration(gameSettings);
        
        TicTacTwoBrain = new TicTacTwoBrain(gameSettings);
        TicTacTwoBrain.AddSettings(GameSetting, Player, PasswordX, PasswordO);
        
        GameName = _gameRepository.SaveGame(TicTacTwoBrain.GameState.ToString(), TicTacTwoBrain.GameSettings.ConfigName);
        

        if (UsesGrid)
        {
            if (TicTacTwoBrain.GameState.PlayersSetting == EGamePlayers.AvA)
            {
                return RedirectToPage("/GridCoordinates", new { gameName = GameName } );                
            }
            
            if (Player == EGamePiece.X)
            {
                return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = PasswordX } );
            }

            return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = PasswordO } );
            

        }
        
        if (TicTacTwoBrain.GameState.PlayersSetting == EGamePlayers.AvA)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName } );                
        }
        
        if (Player == EGamePiece.X)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordX } );
        }
        
        return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordO } );   
    }
    
}
