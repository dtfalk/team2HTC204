using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Refit;
using System.ComponentModel.DataAnnotations;
using Contoso.WebApp.Extensions;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Contoso.WebApp.Pages.Account;
public class LoginModel : PageModel
{
    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string ErrorMessage { get; set; }

    private IContosoAPI _contosoAPI;

    public LoginModel(IContosoAPI contosoAPI)
    {
        _contosoAPI = contosoAPI;
    }

    public void OnGet()
    {
        // This method runs when the page is first loaded (HTTP GET).
    }

    public async Task<IActionResult> OnPost()
    {

        var response = await _contosoAPI.LoginAsync(Input);

        if(!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Login failed.");
            ErrorMessage = "Invalid username or password.";
            return Page();       
        }

        string token = response.Content.token;
        string userName = response.Content.username;

        HttpContext.Session.SetString("AuthToken", token);

        HttpContext.Session.SetString("UserName", userName);
        
        if (userName == "Admin"){
            HttpContext.Session.SetString("IsAdmin", "true");
        } else {
            HttpContext.Session.SetString("IsAdmin", "false");
        }

        return RedirectToPage("/Home/Home");
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}