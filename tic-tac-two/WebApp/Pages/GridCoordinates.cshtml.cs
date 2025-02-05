using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class GridCoordinates : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public GridCoordinates(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] public string? GameName { get; set; }
    [BindProperty(SupportsGet = true)] public string Password { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string ErrorMsg { get; set; } = default!;
    [BindProperty] public int GridStartX { get; set; }
    [BindProperty] public int GridStartY { get; set; }

    public TicTacTwoBrain? TicTacToeBrain { get; set; }
    
    
    public IActionResult OnPost()
    {
        if (!string.IsNullOrWhiteSpace(GameName))
        {
            var savedGameState = _gameRepository.GetSavedGameStateByName(GameName);
            var gameSettings = LoadSavedGame.GetSavedGameSetting(savedGameState);
            TicTacToeBrain = new TicTacTwoBrain(gameSettings);
            TicTacToeBrain.SetGameStateJson(savedGameState);
            
            var result = TicTacToeBrain.GridIsPlacedInBounds(GridStartX, GridStartY);
            if (result)
            {
                TicTacToeBrain.GameState.GridStartX = GridStartX;
                TicTacToeBrain.GameState.GridStartY = GridStartY;
            
                _gameRepository.UpdateGame(GameName, TicTacToeBrain.GetGameStateJson());
                return RedirectToPage("/PlayGame", new { gameName = GameName, password = Password });                
            }

            ErrorMsg = $"Cannot place grid out of bounds. Board: {gameSettings.BoardSizeWidth}x{gameSettings.BoardSizeHeight}, " +
                       $"Grid: {gameSettings.GridSizeWidth}x{gameSettings.GridSizeHeight}.";
            return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = Password, errorMsg = ErrorMsg });


        }

        return Page();
    }


}