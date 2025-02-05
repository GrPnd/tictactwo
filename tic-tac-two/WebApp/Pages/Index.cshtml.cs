using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    
    public IActionResult OnPost()
    { 
        string action = Request.Form["action"]!;
        
        if (action == "newGame")
        {
            return RedirectToPage("/NewGame");            
        }
        if (action == "loadGame")
        {
            return RedirectToPage("/LoadGame");
        }
        if (action == "rules")
        {
            return Redirect("https://gamescrafters.berkeley.edu/games.php?game=tictactwo");
        }
        
        return Page();
    }
}