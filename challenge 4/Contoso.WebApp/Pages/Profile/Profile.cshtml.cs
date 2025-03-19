using System.Threading.Tasks;
using Contoso.WebApp.Data;
using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserProfileModel : PageModel
{
    private readonly IContosoAPI _contosoAPI;

    [BindProperty]
    public UserInfoDto User { get; set; }

    public string ErrorMessage { get; set; }


    public UserProfileModel(IContosoAPI contosoAPI)
    {
       _contosoAPI = contosoAPI;
    }


    public async Task OnGetAsync()
    {
    
        var userInfoResponse = await _contosoAPI.GetUserInfoAsync();

        if (userInfoResponse.IsSuccessStatusCode)
        {
            User = userInfoResponse.Content;
        }
        else
        {
            ErrorMessage = "An error occurred while retrieving user information.";
            return;
        }

        if(HttpContext.Session.GetString("SuccessMessage") != null)
        {
            ViewData["SuccessMessage"] = HttpContext.Session.GetString("SuccessMessage");
            HttpContext.Session.Remove("SuccessMessage");
        }

        User = userInfoResponse.Content;

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var updateResponse = await _contosoAPI.UpdateUserInfoAsync(User);

        if (!updateResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "An error occurred while updating user information.";
            return Page();
        }

        HttpContext.Session.SetString("UserName", User.Name);

        HttpContext.Session.SetString("SuccessMessage", "User information updated successfully.");

        return RedirectToPage();
    }

}
