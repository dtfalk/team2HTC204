using Contoso.Api.Data;
using Contoso.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AccountController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginDto>> Login(UserCredentialsDto userLoginDto)
    {
        var loginDto = await _authenticationService.LoginAsync(userLoginDto);

        if (loginDto == null)
        {
            return BadRequest("Invalid credentials");
        }

        return loginDto;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginDto>> Register(UserDto userDto)
    {
        
        var loginDto = await _authenticationService.RegisterAsync(userDto);

        if (loginDto == null)
        {
            return BadRequest("User already exists");
        }

        return loginDto;
    }
}