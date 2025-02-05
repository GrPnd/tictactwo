using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PasswordVerification : PageModel
{
    private readonly IGameRepository _gameRepository;

    public PasswordVerification(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    [BindProperty(SupportsGet = true)] public string GameName { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public string Message { get; set; } = default!;
    [BindProperty] public string PasswordX { get; set; } = default!;
    [BindProperty] public string PasswordO { get; set; } = default!;
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    
    public void OnGet()
    {
        var savedGameState = _gameRepository.GetSavedGameStateByName(GameName);
        
        var gameSettings = LoadSavedGame.GetSavedGameSetting(savedGameState);
            
        TicTacTwoBrain = new TicTacTwoBrain(gameSettings);
        
        TicTacTwoBrain.SetGameStateJson(savedGameState);
    }

    public IActionResult OnPost()
    {
        var savedGameState = _gameRepository.GetSavedGameStateByName(GameName);
        
        var gameSettings = LoadSavedGame.GetSavedGameSetting(savedGameState);
            
        TicTacTwoBrain = new TicTacTwoBrain(gameSettings);
        
        TicTacTwoBrain.SetGameStateJson(savedGameState);
        
        if (TicTacTwoBrain.GameState.PasswordX == PasswordX && TicTacTwoBrain.GameState.PasswordX != null)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordX });
        }

        if (TicTacTwoBrain.GameState.PasswordO == PasswordO && TicTacTwoBrain.GameState.PasswordO != null)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordO });
        }

        if (TicTacTwoBrain.GameState.PlayersSetting == EGamePlayers.AvA)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName });
        }
        
        Message = "Incorrect password!";
        return Page();
    }
}