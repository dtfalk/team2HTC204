using System.ComponentModel.DataAnnotations;

namespace Contoso.Api.Data;

public class UserDto
{
    public string? Name { get; set; }

    [Required (ErrorMessage = "Email is required")]
    public required string Email { get; set; }

    public string? Address { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string Password { get; set; }
}