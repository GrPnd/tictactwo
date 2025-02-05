using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages;

public class LoadGame : PageModel
{
    private readonly IGameRepository _gameRepository;

    public LoadGame(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public SelectList GameSelectList { get; set; } = default!;
    [BindProperty] public string GameName { get; set; } = default!;
    
    public IActionResult OnGet()
    {
        
        var selectListData = _gameRepository.GetSavedGamesNames();
        
        GameSelectList = new SelectList(selectListData.Select(x => new SelectListItem
        {
            Text = x,  // Display game name
            Value = x   // Use game name as value
        }), "Value", "Text");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        return RedirectToPage("/PasswordVerification", new { gameName = GameName });
    }
}