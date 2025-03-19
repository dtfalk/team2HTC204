using System.Security.Claims;
using System.Threading.Tasks;
using Contoso.Api.Models;
using Contoso.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var userInfo = await _userService.GetUserInfo(userId);

        return Ok(userInfo);
    }


    [HttpPut]
    public async Task<IActionResult> UpdateUserInfo(UserInfoDto userInfo)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _userService.UpdateUserInfo(userInfo, userId);

        return Ok();
    }


}