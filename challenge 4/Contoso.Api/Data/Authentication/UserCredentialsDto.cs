using System.ComponentModel.DataAnnotations;

namespace Contoso.Api.Data;

public class UserCredentialsDto
{
    public  required string Email { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public  required string Password { get; set; }
}