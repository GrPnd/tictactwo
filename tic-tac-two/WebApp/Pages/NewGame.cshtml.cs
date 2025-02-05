using DAL;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class NewGame : PageModel
{
    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;
    
    public NewGame(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
        _configRepository = configRepository;
    }
    
    public SelectList ConfigSelectList { get; set; } = default!;
    [BindProperty] public string? ConfigName { get; set; }
    [BindProperty(SupportsGet = true)] public string? GameName { get; set; }
    [BindProperty] public EGamePlayers SelectedGameSetting { get; set; } = EGamePlayers.PvP;
    [BindProperty] public EGamePiece SelectedPlayerSymbol { get; set; } = EGamePiece.X;
    [BindProperty] public string? PasswordX { get; set; }
    [BindProperty] public string? PasswordO { get; set; }
    [BindProperty] public string? Password { get; set; }
    
    public TicTacTwoBrain TicTacTwoBrain { get; set; } = default!;
    
    public IActionResult OnGet()
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        
        ConfigSelectList = new SelectList(selectListData, "id", "value");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        var selectListData = _configRepository.GetConfigurationNames()
            .Select(name => new {id = name, value = name})
            .ToList();
        
        ConfigSelectList = new SelectList(selectListData, "id", "value");
        
        
        if (SelectedGameSetting == EGamePlayers.PvP)
        {
            if (string.IsNullOrWhiteSpace(PasswordX) || string.IsNullOrWhiteSpace(PasswordO))
            {
                ModelState.AddModelError(string.Empty, "Both passwords are required for Player vs Player mode.");
                return Page();
            }
        }
        else if (SelectedGameSetting == EGamePlayers.PvA)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Password is required for Player vs AI mode.");
                return Page();
            }

            if (SelectedPlayerSymbol == EGamePiece.X)
            {
                PasswordX = Password;
            }
            else
            {
                PasswordO = Password;
            }
        }


        GameName = CreateGame(ConfigName);
        
        if (ConfigName == "Custom")
        {
            return RedirectToPage("/ConfigureCustomGame", new { gameSetting = SelectedGameSetting, player = SelectedPlayerSymbol.ToString(), passwordX = PasswordX, passwordO = PasswordO } );
        }
        

        if (TicTacTwoBrain.GameState.GameSettings.UsesGrid)
        {
            if (SelectedPlayerSymbol == EGamePiece.X)
            {
                return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = PasswordX } );
            }
            return RedirectToPage("/GridCoordinates", new { gameName = GameName, password = PasswordO } );
        }

        if (SelectedPlayerSymbol == EGamePiece.X)
        {
            return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordX } );
        }
        
        return RedirectToPage("/PlayGame", new { gameName = GameName, password = PasswordO } );
        
    }


    public string CreateGame(string configName)
    {
        var gameSettings = _configRepository.GetConfigurationByName(ConfigName);
        TicTacTwoBrain = new TicTacTwoBrain(gameSettings);
        
        TicTacTwoBrain.AddSettings(SelectedGameSetting, SelectedPlayerSymbol, PasswordX, PasswordO);
        
        GameName = _gameRepository.SaveGame(TicTacTwoBrain.GetGameStateJson(), ConfigName);
        return GameName;
    }
}