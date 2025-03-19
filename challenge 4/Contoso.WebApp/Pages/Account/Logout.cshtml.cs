using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{

    public IActionResult OnGet()
    {

        HttpContext.Session.Remove("AuthToken");
        HttpContext.Session.Clear();
        
        return RedirectToPage("/Home/Home");
    }
}